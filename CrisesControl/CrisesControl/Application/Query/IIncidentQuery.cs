
using CrisesControl.Api.Application.Commands.Incidents.AddNotes;
using CrisesControl.Api.Application.Commands.Incidents.CheckUserSOS;
using CrisesControl.Api.Application.Commands.Incidents.GetActiveIncidentBasic;
using CrisesControl.Api.Application.Commands.Incidents.GetAttachments;
using CrisesControl.Api.Application.Commands.Incidents.GetCallToAction;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentSOSRequest;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentTaskNotes;
using CrisesControl.Api.Application.Commands.Incidents.GetIndidentTimeline;
using CrisesControl.Api.Application.Commands.Incidents.UpdateSOS;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Query
{
    public interface IIncidentQuery
    {
        List<IncidentList> GetAllCompanyIncident(int userId);
        List<IncidentTypeReturn> GetCompanyIncidentType(int companyId);
        List<AffectedLocation> GetAffectedLocations(int companyId, string locationType);
        List<AffectedLocation> GetIncidentLocations(int companyId, int incidentActivationId);
        List<CommsMethods> GetIncidentComms(int itemID, string type);
        IncidentDetails GetCompanyIncidentById(int companyId, int incidentId, string userStatus);
        DataTablePaging GetAllActiveCompanyIncident(string? status, DataTableAjaxPostModel pagedRequest);
        Task<GetIncidentTaskNotesResponse> GetIncidentTaskNotes(GetIncidentTaskNotesRequest request);
        Task<CheckUserSOSResponse> CheckUserSOS(CheckUserSOSRequest request);
        Task<AddNotesResponse> AddNotes(AddNotesRequest request);
        Task<GetActiveIncidentBasicResponse> GetActiveIncidentBasic(GetActiveIncidentBasicRequest request);
        Task<GetAttachmentsResponse> GetAttachments(GetAttachmentsRequest request);
        Task<GetCallToActionResponse> GetCallToAction(GetCallToActionRequest request);
        Task<GetIncidentSOSRequestResponse> GetIncidentSOSRequest(GetIncidentSOSRequest request);
        Task<GetIndidentTimelineResponse> GetIndidentTimeline(GetIndidentTimelineRequest request);
        Task<UpdateSOSResponse> UpdateSOS(UpdateSOSRequest request);


    }
}
