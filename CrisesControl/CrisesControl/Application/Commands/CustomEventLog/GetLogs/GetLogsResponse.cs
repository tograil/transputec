using CrisesControl.Core.CustomEventLog;
using CrisesControl.Core.Users;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.GetLogs
{
    public class GetLogsResponse
    {
        public List<EventLogListing> Data { get; set; }
    }
}
