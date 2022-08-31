using CrisesControl.Core.CustomEventLog;
using CrisesControl.Core.Users;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.GetMessageLog
{
    public class GetMessageLogResponse
    {
        public List<EventMessageLog> Data { get; set; }
    }
}
