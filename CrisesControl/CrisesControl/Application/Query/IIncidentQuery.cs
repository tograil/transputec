
using CrisesControl.Api.Application.Commands.Incidents.AddIncidentAction;
using CrisesControl.Api.Application.Commands.Incidents.AddIncidentAsset;
using CrisesControl.Api.Application.Commands.Incidents.AddIncidentType;
using CrisesControl.Api.Application.Commands.Incidents.AddNotes;
using CrisesControl.Api.Application.Commands.Incidents.CheckUserSOS;
using CrisesControl.Api.Application.Commands.Incidents.DeleteCompanyIncident;
using CrisesControl.Api.Application.Commands.Incidents.DeleteCompanyIncidentAction;
using CrisesControl.Api.Application.Commands.Incidents.DeleteIncidentAsset;
using CrisesControl.Api.Application.Commands.Incidents.GetActiveIncidentBasic;
using CrisesControl.Api.Application.Commands.Incidents.GetActiveIncidentDetailsById;
using CrisesControl.Api.Application.Commands.Incidents.GetAttachments;
using CrisesControl.Api.Application.Commands.Incidents.GetCallToAction;
using CrisesControl.Api.Application.Commands.Incidents.GetCMDMessage;
using CrisesControl.Api.Application.Commands.Incidents.GetCompanySOS;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentAction;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentAsset;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentByName;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentEntityRecipient;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentLibrary;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentMapLocations;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentMessage;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentRecipientEntity;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentSOSRequest;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentTaskNotes;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentTimeline;
using CrisesControl.Api.Application.Commands.Incidents.GetSOSIncident;
using CrisesControl.Api.Application.Commands.Incidents.IncidentStatusUpdate;
using CrisesControl.Api.Application.Commands.Incidents.SaveIncidentJob;
using CrisesControl.Api.Application.Commands.Incidents.SaveIncidentParticipants;
using CrisesControl.Api.Application.Commands.Incidents.SegregationLinks;
using CrisesControl.Api.Application.Commands.Incidents.TestWithMe;
using CrisesControl.Api.Application.Commands.Incidents.UpdateCompanyIncident;
using CrisesControl.Api.Application.Commands.Incidents.UpdateCompanyIncidentAction;
using CrisesControl.Api.Application.Commands.Incidents.UpdateSegregationLink;
using CrisesControl.Api.Application.Commands.Incidents.UpdateSOS;
using CrisesControl.Api.Application.Commands.Incidents.UpdateSOSIncident;
using CrisesControl.Api.Application.Commands.Incidents.GetAllCompanyIncident;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Messages;
using CrisesControl.Api.Application.Commands.Incidents.GetAffectedLocations;
using CrisesControl.Api.Application.Commands.Incidents.GetAllActiveCompanyIncident;

namespace CrisesControl.Api.Application.Query
{
    public interface IIncidentQuery
    {
     
        Task<GetAllCompanyIncidentResponse> GetAllCompanyIncident(GetAllCompanyIncidentRequest request);
        List<IncidentTypeReturn> GetCompanyIncidentType(int companyId);
        Task<GetAffectedLocationsResponse> GetAffectedLocations(GetAffectedLocationsRequest request);
        List<AffectedLocation> GetIncidentLocations(int companyId, int incidentActivationId);
        List<CommsMethods> GetIncidentComms(int itemID, string type);
        IncidentDetails GetCompanyIncidentById(int companyId, int incidentId, string userStatus);
        Task<GetAllActiveCompanyIncidentResponse> GetAllActiveCompanyIncident(GetAllActiveCompanyIncidentRequest request);
        Task<GetIncidentTaskNotesResponse> GetIncidentTaskNotes(GetIncidentTaskNotesRequest request);
        Task<CheckUserSOSResponse> CheckUserSOS(CheckUserSOSRequest request);
        Task<AddNotesResponse> AddNotes(AddNotesRequest request);
        Task<GetActiveIncidentBasicResponse> GetActiveIncidentBasic(GetActiveIncidentBasicRequest request);
        Task<GetAttachmentsResponse> GetAttachments(GetAttachmentsRequest request);
        Task<GetCallToActionResponse> GetCallToAction(GetCallToActionRequest request);
        Task<GetIncidentSOSRequestResponse> GetIncidentSOSRequest(GetIncidentSOSRequest request);
        Task<GetIncidentTimelineResponse> GetIncidentTimeline(GetIncidentTimelineRequest request);
        Task<UpdateSOSResponse> UpdateSOS(UpdateSOSRequest request);
        Task<AddIncidentTypeResponse> AddIncidentType(AddIncidentTypeRequest request);
        Task<AddIncidentActionResponse> AddIncidentAction(AddIncidentActionRequest request);
        Task<AddIncidentAssetResponse> AddIncidentAsset(AddIncidentAssetRequest request);
        Task<UpdateCompanyIncidentResponse> UpdateCompanyIncident(UpdateCompanyIncidentRequest request);
        Task<UpdateCompanyIncidentActionResponse> UpdateCompanyIncidentAction(UpdateCompanyIncidentActionRequest request);
        Task<DeleteCompanyIncidentResponse> DeleteCompanyIncident(DeleteCompanyIncidentRequest request);
        Task<DeleteCompanyIncidentActionResponse> DeleteCompanyIncidentAction(DeleteCompanyIncidentActionRequest request);
        Task<DeleteIncidentAssetResponse> DeleteIncidentAsset(DeleteIncidentAssetRequest request);
        Task<GetActiveIncidentDetailsByIdResponse> GetActiveIncidentDetailsById(GetActiveIncidentDetailsByIdRequest request);
        Task<IncidentStatusUpdateResponse> IncidentStatusUpdate(IncidentStatusUpdateRequest request);
        Task<GetIncidentLibraryResponse> GetIncidentLibrary(GetIncidentLibraryRequest request);
        Task<GetIncidentMessageResponse> GetIncidentMessage(GetIncidentMessageRequest request);
        Task<GetIncidentActionResponse> GetIncidentAction(GetIncidentActionRequest request);
        Task<GetIncidentAssetResponse> GetIncidentAsset(GetIncidentAssetRequest request);
        Task<GetCompanySOSResponse> GetCompanySOS(GetCompanySOSRequest request);
        Task<UpdateSOSIncidentResponse> UpdateSOSIncident(UpdateSOSIncidentRequest request);
        Task<TestWithMeResponse> TestWithMe(TestWithMeRequest request);
        Task<GetCMDMessageResponse> GetCMDMessage(GetCMDMessageRequest request);
        Task<GetIncidentMapLocationsResponse> GetIncidentMapLocations(GetIncidentMapLocationsRequest request);
        Task<SaveIncidentParticipantsResponse> SaveIncidentParticipants(SaveIncidentParticipantsRequest request);
        Task<GetIncidentByNameResponse> GetIncidentByName(GetIncidentByNameRequest request);
        Task<SaveIncidentJobResponse> SaveIncidentJob(SaveIncidentJobRequest request);
        Task<SegregationLinksResponse> SegregationLinks(SegregationLinksRequest request);
        Task<UpdateSegregationLinkResponse> UpdateSegregationLink(UpdateSegregationLinkRequest request);
        Task<GetSOSIncidentResponse> GetSOSIncident(GetSOSIncidentRequest request);
        Task<GetIncidentEntityRecipientResponse> GetIncidentEntityRecipient(GetIncidentEntityRecipientRequest request);
        Task<GetIncidentRecipientEntityResponse> GetIncidentRecipientEntity(GetIncidentRecipientEntityRequest request);

    }
}
