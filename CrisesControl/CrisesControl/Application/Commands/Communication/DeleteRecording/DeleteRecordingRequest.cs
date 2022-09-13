using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.DeleteRecording
{
    public class DeleteRecordingRequest:IRequest<DeleteRecordingResponse>
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string RecordingSid { get; set; }
        public string DataCenter { get; set; }
    }
}
