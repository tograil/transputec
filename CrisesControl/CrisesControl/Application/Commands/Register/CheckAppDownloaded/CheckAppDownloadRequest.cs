using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.CheckAppDownloaded
{
    public class CheckAppDownloadRequest:IRequest<CheckAppDownloadResponse>
    {
        public int UserId { get; set; }
    }
}
