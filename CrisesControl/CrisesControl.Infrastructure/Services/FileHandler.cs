using Azure;
using Azure.Storage;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Sas;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.AuditLog.Services;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public class FileHandler
    {
        private string connectionString;
        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DBCommon DBC;
        private string hostingEnv;
        private readonly IAuditLogService _auditLogService;
        public FileHandler(CrisesControlContext context,  IHttpContextAccessor httpContextAccessor)
        {
            connectionString = DBC.Getconfig("AzureFileShareConnection");
            hostingEnv = DBC.Getconfig("HostingEnvironment");
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            DBC = new DBCommon(_context, _httpContextAccessor);
        }

        public Uri GetFileSasUri(string shareName, string filePath, DateTime expiration, ShareFileSasPermissions permissions)
        {
            // Get the account details from app settings

            string accountName = DBC.Getconfig("StorageAccountName");
            string accountKey = DBC.Getconfig("StorageAccountKey");

            ShareSasBuilder fileSAS = new ShareSasBuilder()
            {
                ShareName = shareName,
                FilePath = filePath,

                // Specify an Azure file resource
                Resource = "f",

                // Expires in 24 hours
                ExpiresOn = expiration
            };

            // Set the permissions for the SAS
            fileSAS.SetPermissions(permissions);

            // Create a SharedKeyCredential that we can use to sign the SAS token
            StorageSharedKeyCredential credential = new StorageSharedKeyCredential(accountName, accountKey);

            // Build a SAS URI
            UriBuilder fileSasUri = new UriBuilder($"https://{accountName}.file.core.windows.net/{fileSAS.ShareName}/{fileSAS.FilePath}");
            fileSasUri.Query = fileSAS.ToSasQueryParameters(credential).ToString();

            // Return the URI
            return fileSasUri.Uri;
        }

        public async Task CopyFileAsync(string sourceShareName, string sourceFilePath, string destiShareName, string destFilePath)
        {
            // Get the connection string from app settings
            string connectionString = DBC.Getconfig("AzureFileShareConnection");

            // Get a reference to the file we created previously
            ShareFileClient sourceFile = new ShareFileClient(connectionString, sourceShareName, sourceFilePath);

            // Ensure that the source file exists
            if (sourceFile.Exists())
            {
                // Get a reference to the destination file

                ShareFileClient destFile = new ShareFileClient(connectionString, destiShareName, destFilePath);

                // Start the copy operation

                destFile.StartCopy(sourceFile.Uri);

                UpdateFileMetaData(destFile, "Content-Type", MimeList.GetMimeType(destFilePath));
                UpdateFileMetaData(destFile, "Cache-Control", "no-cache, no-store, must-revalidate");

                if (destFile.Exists())
                {
                    DBC.CreateLog("INFO", $"{sourceFilePath} copied to {destFile.Uri}");
                    Console.WriteLine($"{sourceFilePath} copied to {destFile.Uri}");
                }
            }
        }

        public bool FileExists(string filePath, string shareName, string directoryPath, bool rootDirectory = false)
        {

            try
            {
                if (hostingEnv == "AZURE")
                {
                    ShareClient share = new ShareClient(connectionString, shareName);
                    ShareDirectoryClient directory = share.GetRootDirectoryClient();
                    if (!rootDirectory)
                    {
                        directory = share.GetDirectoryClient(@directoryPath.Replace("\\", "/"));
                        ShareFileClient file = directory.GetFileClient(filePath);
                        return file.Exists();
                    }
                }
                else
                {
                    return File.Exists(directoryPath + filePath);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public void DownloaFileFromAzure(string shareName, string dirName, string fileName, string savePath, bool rootDirectory = true)
        {
            try
            {
                ShareDirectoryClient directory;
                ShareClient share = new ShareClient(connectionString, shareName);
                directory = share.GetRootDirectoryClient();
                if (!rootDirectory)
                {
                    directory = share.GetDirectoryClient(dirName.Replace("\\", "/"));
                }
                ShareFileClient file = directory.GetFileClient(fileName);

                UpdateFileMetaData(file, "Content-Type", MimeList.GetMimeType(fileName));
                UpdateFileMetaData(file, "Cache-Control", "no-cache, no-store, must-revalidate");

                // Download the file
                ShareFileDownloadInfo download = file.Download();
                using (FileStream stream = File.OpenWrite(savePath))
                {
                    download.Content.CopyTo(stream);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> UploadToAzure(string shareName, string dirName, string fileName, FileStream fileStream)
        {
            try
            {
                string connectionString = DBC.Getconfig("AzureFileShareConnection");

                const int uploadLimit = 4194304;

                fileStream.Seek(0, SeekOrigin.Begin);

                ShareClient share = new ShareClient(connectionString, shareName);
                if (!share.Exists())
                    share.Create();

                // Get a reference to a directory and create it
                ShareDirectoryClient directory = share.GetDirectoryClient(dirName.Replace("\\", "/"));
                if (!directory.Exists())
                {
                    directory.Create();
                }

                // Get a reference to a file and upload it
                ShareFileClient file = directory.GetFileClient(fileName);
                //using (FileStream stream = File.OpenRead(filePath)) {
                file.Create(fileStream.Length);

                UpdateFileMetaData(file, "Content-Type", MimeList.GetMimeType(fileName));
                UpdateFileMetaData(file, "Cache-Control", "no-cache, no-store, must-revalidate");

                if (fileStream.Length <= uploadLimit)
                {
                    file.UploadRange(new HttpRange(0, fileStream.Length), fileStream);
                    return true;
                }
                else
                {
                    int bytesRead;
                    long index = 0;
                    byte[] buffer = new byte[uploadLimit];

                    // Stream is larger than the limit so we need to upload in chunks
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        // Create a memory stream for the buffer to upload
                        using (MemoryStream ms = new MemoryStream(buffer, 0, bytesRead))
                        {
                            file.UploadRange(ShareFileRangeWriteType.Update, new HttpRange(index, ms.Length), ms);
                            index += ms.Length; // increment the index to the account for bytes already written
                        }
                    }
                    return true;
                }

                //}
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }

        public void UpdateFileMetaData(ShareFileClient file, string PropertyName, string ProperValue)
        {
            try
            {
                Dictionary<string, string> meta = new Dictionary<string, string>();
                meta.Add(PropertyName, MimeList.GetMimeType(ProperValue));
                file.SetMetadata(meta);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
