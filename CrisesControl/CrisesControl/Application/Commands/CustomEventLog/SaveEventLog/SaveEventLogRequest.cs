using CrisesControl.Core.CustomEventLog;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.SaveEventLog
{
    public class SaveEventLogRequest : IRequest<SaveEventLogResponse>
    {
        public EventLogEntry IP { get; set; }
        public int UserID { get; set; }
        public string TimeZoneId { get; set; }

    }
}
