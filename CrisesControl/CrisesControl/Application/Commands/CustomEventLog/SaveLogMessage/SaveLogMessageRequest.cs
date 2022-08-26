using MediatR;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.SaveLogMessage
{
    public class SaveLogMessageRequest : IRequest<SaveLogMessageResponse>
    {
        public int EventLogId { get; set; }
        public int MessageId { get; set; }
        public string MessageText { get; set; }
        public int UserId { get; set; }
        public string TimeZoneId { get; set; }
    }
}
