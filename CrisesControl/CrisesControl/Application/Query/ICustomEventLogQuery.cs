using CrisesControl.Api.Application.Commands.CustomEventLog.GetEventLog;
using CrisesControl.Api.Application.Commands.CustomEventLog.GetEventLogHeader;
using CrisesControl.Api.Application.Commands.CustomEventLog.GetLogs;
using CrisesControl.Api.Application.Commands.CustomEventLog.GetMessageLog;

namespace CrisesControl.Api.Application.Query
{
    public interface ICustomEventLogQuery
    {
        Task<GetEventLogResponse> GetEventLog(GetEventLogRequest request);
        Task<GetEventLogHeaderResponse> GetEventLogHeader(GetEventLogHeaderRequest request);
        Task<GetLogsResponse> GetLogs(GetLogsRequest request);
        Task<GetMessageLogResponse> GetMessageLog(GetMessageLogRequest request);
    }
}
