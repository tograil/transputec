using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Register.CheckAppDownloaded
{
    public class CheckAppDownloadResponse
    {
        public string Message { get; set; }
        public UserDevice Data { get; set; }
    }
}
