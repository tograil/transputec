using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelConfRecording
{
    public class HandelConfRecordingRequest:IRequest<HandelConfRecordingResponse>
    {
       
        public string ConferenceSid { get; set; }      
      
        public int Duration { get; set; }
        public string RecordingUrl { get; set; }
        public string RecordingSid { get; set; }
        public int RecordingFileSize { get; set; }
        public string RecordingStatus { get; set; }
    }
}
