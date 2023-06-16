using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.DownloadRecording
{
    public class DownloadRecordingRequest:IRequest<DownloadRecordingResponse>
    {
        public string  FileName { get; set; }
    }
}
