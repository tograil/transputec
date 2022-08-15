using Azure.Core;
using CrisesControl.Core.FileService.Repositories;
using MediatR;
using System.Net;

namespace CrisesControl.Api.Application.Commands.FileService.UploadToAzure
{
    public class UploadToAzureHandler : IRequestHandler<UploadToAzureRequest, HttpResponseMessage>
    {
        private readonly IFileServiceRepository _fileServiceRepository;
        private readonly ILogger<UploadToAzureHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UploadToAzureHandler(IFileServiceRepository fileServiceRepository, ILogger<UploadToAzureHandler> logger)
        {
            this._fileServiceRepository = fileServiceRepository;
            this._logger = logger;
        }
        public async Task<HttpResponseMessage> Handle(UploadToAzureRequest request, CancellationToken cancellationToken)
        {
           
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.BadRequest); 
            HttpRequestMessage requestMessage;
            if (!_httpContextAccessor.HttpContext.Request.HasFormContentType)
            {
                result = new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType);
            }
           
            string shareName = string.Empty;
            string upload_dir = string.Empty;

            var httpRequest = _httpContextAccessor.HttpContext?.Request;
            foreach (var key in httpRequest.Form.Keys)
            {
                foreach (var val in httpRequest.Form.ToDictionary(x => x.Key, x => x.Value.ToString()))
                {
                    if (key == "upload_dir")
                    {
                        upload_dir = val.ToString();
                    }
                    else if (key == "share_name")
                    {
                        shareName = val.ToString();
                    }
                }
            }

            if (httpRequest.Form.Files.Count > 0)
            {
                var filePath = Path.GetTempFileName();
                foreach (var file in httpRequest.Form.Files)
                {
                    using (var inputStream = new FileStream(filePath, FileMode.Create))
                    {
                        // read file to stream
                        await file.CopyToAsync(inputStream);
                        // stream to byte array
                        byte[] array = new byte[inputStream.Length];
                        inputStream.Seek(0, SeekOrigin.Begin);
                        inputStream.Read(array, 0, array.Length);
                         string fileName = file.FileName;
                        var boolval = await _fileServiceRepository.UploadFileToFileShare(shareName, upload_dir, fileName, inputStream);
                    }
                }
            }
            else
            {
                result = new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return result;
        }
    }
}
