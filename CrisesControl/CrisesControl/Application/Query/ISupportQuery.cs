using CrisesControl.Api.Application.Commands.Support.GetIncidentStats;
using CrisesControl.Api.Application.Commands.Support.ActiveIncidentTasks;
using CrisesControl.Api.Application.Commands.Support.GetIncidentData;
using CrisesControl.Api.Application.Commands.Support.GetIncidentReportDetails;
using CrisesControl.Api.Application.Commands.Support.GetIncidentMessageAck;
using CrisesControl.Api.Application.Commands.Support.GetUser;
using CrisesControl.Core.Compatibility;

namespace CrisesControl.Api.Application.Query
{
    public interface ISupportQuery
    {
        Task<GetIncidentDataResponse> GetIncidentData(GetIncidentDataRequest request);
        Task<GetIncidentStatsResponse> GetIncidentStats(GetIncidentStatsRequest request);
        Task<GetIncidentReportDetailsResponse> GetIncidentReportDetails(GetIncidentReportDetailsRequest request);
        Task<ActiveIncidentTasksResponse> ActiveIncidentTasks(ActiveIncidentTasksRequest request);
        Task<GetIncidentMessageAckResponse> GetIncidentMessageAck(GetIncidentMessageAckRequest request);
        Task<GetUserResponse> GetUser(GetUserRequest request);
    }
}
