
using Azure;
using Azure.Storage.Files.Shares;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.FileService.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class FileServiceRepository:IFileServiceRepository
    {
        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDBCommonRepository _DBC;
        public FileServiceRepository(CrisesControlContext context, IDBCommonRepository DBC, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _DBC =  DBC;

        }
       

        public async Task<bool> UploadFileToFileShare(string shareName, string dirName, string fileName, Stream fileStream)
        {
            try
            {
                string connectionString =await _DBC.Getconfig("AzureFileShareConnection");
                //string connectionString = "DefaultEndpointsProtocol=https;AccountName=crisescontroluae;AccountKey=ymMQovjoq3BHsyxRa6PJ9JiJ3OaFjSLASmyvstM0QnLKee3QKvPKBoGpM+AmJalb7ZKZmkT9DYTL+vRoKMV6uA==;EndpointSuffix=core.windows.net";
                const int uploadLimit = 4194304;

                fileStream.Seek(0, SeekOrigin.Begin);

                ShareClient share = new ShareClient(connectionString, shareName);
                if (!share.Exists())
                    share.Create();

                // Get a reference to a directory and create it
                ShareDirectoryClient directory = share.GetDirectoryClient(dirName);
                if (!directory.Exists())
                {
                    directory.Create();
                }

                // Get a reference to a file and upload it
                ShareFileClient file = directory.GetFileClient(fileName);
                //using (FileStream stream = File.OpenRead(filePath)) {
                file.Create(fileStream.Length);
                if (fileStream.Length <= uploadLimit)
                {
                    await file.UploadRangeAsync(new HttpRange(0, fileStream.Length), fileStream).ConfigureAwait(false);
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
                            await file.UploadRangeAsync(Azure.Storage.Files.Shares.Models.ShareFileRangeWriteType.Update, new HttpRange(index, ms.Length), ms).ConfigureAwait(false);
                            index += ms.Length; // increment the index to the account for bytes already written
                        }
                    }
                    return true;
                }

                //}
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}
