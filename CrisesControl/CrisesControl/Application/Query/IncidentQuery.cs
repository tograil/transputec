using AutoMapper;
using CrisesControl.Api.Application.Commands.Incidents.GetAllCompanyIncident;
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
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Models;
using CrisesControl.Core.Reports;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Api.Application.Query;

public class IncidentQuery : IIncidentQuery
{
    private readonly IActiveIncidentRepository _activeIncidentRepository;
    private readonly IIncidentRepository _incidentRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;
    private readonly IPaging _paging;

    public IncidentQuery(IActiveIncidentRepository activeIncidentRepository,
        IIncidentRepository incidentRepository,
        ICurrentUser currentUser, IMapper mapper, IPaging paging)
    {
        _activeIncidentRepository = activeIncidentRepository;
        _incidentRepository = incidentRepository;
        _currentUser = currentUser;
        _mapper = mapper;
        _paging = paging;
    }

    public async Task<GetAllCompanyIncidentResponse> GetAllCompanyIncident(GetAllCompanyIncidentRequest request)
    {
        ///return _incidentRepository.GetCompanyIncident(_currentUser.CompanyId, userId > 0 ? userId : _currentUser.UserId);
        return null;
    }

    public List<IncidentTypeReturn> GetCompanyIncidentType(int companyId)
    {
        return _incidentRepository.CompanyIncidentType(companyId);
    }

    public List<AffectedLocation> GetAffectedLocations(int companyId, string locationType)
    {
        return _incidentRepository.GetAffectedLocation(companyId, locationType);
    }

    public List<AffectedLocation> GetIncidentLocations(int companyId, int incidentActivationId)
    {
        return _incidentRepository.GetIncidentLocation(companyId, incidentActivationId);
    }

    public List<CommsMethods> GetIncidentComms(int itemID, string type)
    {
        return _incidentRepository.GetIncidentComms(itemID, type);
    }

    public IncidentDetails GetCompanyIncidentById(int companyId, int incidentId, string userStatus)
    {
        return _incidentRepository.GetIncidentById(companyId, _currentUser.UserId, incidentId, userStatus);
    }

    public DataTablePaging GetAllActiveCompanyIncident(string? status)
    {
        string OrderBy = _paging.Order != null ? _paging.Order : "Name";
        string OrderDir = _paging.Dir != null ? _paging.Dir : "asc";
        
        string Status = status != null ? status : "1,2,3,4";

        int totalRecord = 0;
        DataTablePaging rtn = new DataTablePaging();
        rtn.Draw = _paging.Draw;

        var ActIncidentDtl = _activeIncidentRepository.GetCompanyActiveIncident(_currentUser.CompanyId, _currentUser.UserId, Status, _paging.Start, _paging.Length, _paging.Search, OrderBy, OrderDir);

        if (ActIncidentDtl != null)
        {
            totalRecord = ActIncidentDtl.Count;
            rtn.RecordsFiltered = ActIncidentDtl.Count;
            rtn.Data = ActIncidentDtl;
        }

        var TotalList = _activeIncidentRepository.GetCompanyActiveIncident(_currentUser.CompanyId, _currentUser.UserId, Status, 0, int.MaxValue, "", "IncidentActivationId", "asc");

        if (TotalList != null)
        {
            totalRecord = TotalList.Count;
        }

        rtn.RecordsTotal = totalRecord;

        return rtn;
    }

    public async Task<GetIncidentTaskNotesResponse> GetIncidentTaskNotes(GetIncidentTaskNotesRequest request)
    {
        var groups = await _incidentRepository.GetNotes(request.ObjectID, request.NoteType, request.GetAttachments, request.AttachmentType, _currentUser.CompanyId);
        var result = _mapper.Map<List<IncidentTask>>(groups);
        var response = new GetIncidentTaskNotesResponse();
        if (result != null)
        {
            response.result = result;
            response.Message = "Duplicate Deprtment.";
        }
        else
        {
            response.Message = "No record found.";
        }
        return response;
    }

    public async Task<CheckUserSOSResponse> CheckUserSOS(CheckUserSOSRequest request)
    {
        var sos = await _incidentRepository.CheckUserSOS(request.ActiveIncidentId, _currentUser.UserId);
        var result = _mapper.Map<int>(sos);
        var response = new CheckUserSOSResponse();
        if (result > 0)
        {
            response.result = result;
            return response;

        }
        return response;

    }

    public async Task<AddNotesResponse> AddNotes(AddNotesRequest request)
    {
        //List<Attachment> Attachments = new List<Attachment>();
        var groups = await _incidentRepository.AddIncidentNote(request.ActiveIncidentID, request.Note, request.Attachments, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
        var result = _mapper.Map<bool>(groups);
        var response = new AddNotesResponse();
        if (result)
        {
            response.result = result;
            response.Message = "Added";
        }
        else
        {
            response.Message = "Not added";
        }
        return response;
    }

    public async Task<GetActiveIncidentBasicResponse> GetActiveIncidentBasic(GetActiveIncidentBasicRequest request)
    {
        var basic = await _incidentRepository.GetActiveIncidentBasic(_currentUser.CompanyId, request.IncidentActivationId);
        var result = _mapper.Map<UpdateIncidentStatusReturn>(basic);
        var response = new GetActiveIncidentBasicResponse();
        if (result != null)
        {
            response.result = result;
            return response;

        }

        return response;
    }

    public async Task<GetAttachmentsResponse> GetAttachments(GetAttachmentsRequest request)
    {
        try
        {
            var groups = await _incidentRepository.GetAttachments(request.ObjectId, request.AttachmentsType);
            var result = _mapper.Map<List<Attachment>>(groups);
            var response = new GetAttachmentsResponse();
            if (result != null)
            {
                response.result = result;
                response.Message = "Data loaded";
            }
            else
            {
                response.Message = "No Data found";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetCallToActionResponse> GetCallToAction(GetCallToActionRequest request)
    {
        try
        {
            var groups = await _incidentRepository.GetCallToAction(request.ActiveIncidentId, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
            var result = _mapper.Map<CallToAction>(groups);
            var response = new GetCallToActionResponse();
            if (result != null)
            {
                response.Data = result;

            }
            else
            {
                response.Data = result;
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetIncidentSOSRequestResponse> GetIncidentSOSRequest(GetIncidentSOSRequest request)
    {
        try
        {
            var groups = await _incidentRepository.GetIncidentSOSRequest(request.IncidentActivationId);
            var result = _mapper.Map<List<IncidentSOSRequest>>(groups);
            var response = new GetIncidentSOSRequestResponse();
            if (result != null)
            {
                response.Data = result;

            }
            else
            {
                response.Data = result;
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetIncidentTimelineResponse> GetIncidentTimeline(GetIncidentTimelineRequest request)
    {
        try
        {
            var groups = await _incidentRepository.GetIncidentTimeline(request.IncidentActivationId, _currentUser.CompanyId, _currentUser.UserId);
            var result = _mapper.Map<List<IncidentMessagesRtn>>(groups);
            var response = new GetIncidentTimelineResponse();
            if (result != null)
            {
                response.Data = result;

            }
            else
            {
                response.Data = result;
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<UpdateSOSResponse> UpdateSOS(UpdateSOSRequest request)
    {
        try
        {
            var groups = await _incidentRepository.UpdateSOS(request.SOSAlertID, request.UserID, request.SOSClosureNotes, request.CloseSOS, request.CloseAllSOS,
                        request.MultiNotes, request.CaseNoteIDs, request.CloseSOSIncident, request.ActiveIncidentID, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
            var result = _mapper.Map<bool>(groups);
            var response = new UpdateSOSResponse();
            if (result)
            {
                response.Message = "Updated SOS";

            }
            else
            {
                response.Message = "SOS Not Updated";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<AddIncidentTypeResponse> AddIncidentType(AddIncidentTypeRequest request)
    {
        try
        {
            var typeId = await _incidentRepository.UpdateIncidentType(request.Name, request.IncidentTypeId, _currentUser.UserId, _currentUser.CompanyId);
            var result = _mapper.Map<int>(typeId);
            var response = new AddIncidentTypeResponse();
            if (result > 0)
            {
                var IncidentTypes = _incidentRepository.CompanyIncidentType(_currentUser.CompanyId);
                response.incidentTypes = IncidentTypes;
                response.TypeID = result;
                response.Message = "Added";

            }
            else
            {
                response.Message = "No data found.";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<AddIncidentActionResponse> AddIncidentAction(AddIncidentActionRequest request)
    {
        try
        {
            var actionLsts = await _incidentRepository.AddIncidentActions(_currentUser.CompanyId, request.IncidentId, request.Title, request.ActionDescription, request.IncidentParticipants, request.UsersToNotify,
                        request.Status, _currentUser.UserId, _currentUser.TimeZone);
            var result = _mapper.Map<List<ActionLsts>>(actionLsts);
            var response = new AddIncidentActionResponse();
            if (result != null)
            {
                response.Data = actionLsts;

            }
            else
            {
                response.Data = null;
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<AddIncidentAssetResponse> AddIncidentAsset(AddIncidentAssetRequest request)
    {
        try
        {
            var incidentAssets = await _incidentRepository.AddIncidentAssets(_currentUser.CompanyId, request.IncidentId, request.LinkedAssetId, _currentUser.UserId, _currentUser.TimeZone);
            var result = _mapper.Map<List<IncidentAssets>>(incidentAssets);
            var response = new AddIncidentAssetResponse();
            if (result != null)
            {
                response.Data = incidentAssets;
            }
            else
            {
                response.Data = null;
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<UpdateCompanyIncidentResponse> UpdateCompanyIncident(UpdateCompanyIncidentRequest request)
    {
        try
        {
            var incidentId = await _incidentRepository.UpdateCompanyIncidents(_currentUser.CompanyId, request.IncidentId, request.IncidentIcon, request.Name,
                        request.Description, request.PlanAssetID, request.IncidentTypeId, request.Severity, request.Status,
                        request.NumberOfKeyHolders, _currentUser.UserId, _currentUser.TimeZone, request.UpdIncidentKeyHldLst, request.AudioAssetId,
                        request.TrackUser, request.SilentMessage, request.AckOptions, request.MessageMethod, request.CascadePlanID, request.Groups, request.IncidentKeyholder);
            var result = _mapper.Map<int>(incidentId);
            var response = new UpdateCompanyIncidentResponse();
            if (result > 0)
            {
                response.Result = incidentId;
            }
            else
            {
                response.Result = 0;
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<UpdateCompanyIncidentActionResponse> UpdateCompanyIncidentAction(UpdateCompanyIncidentActionRequest request)
    {
        try
        {
            var incidentId = await _incidentRepository.UpdateCompanyIncidentActions(_currentUser.CompanyId, request.IncidentId, request.IncidentActionId,
                        request.Title, request.ActionDescription, request.IncidentParticipants, request.UsersToNotify,
                        request.Status, _currentUser.UserId, _currentUser.TimeZone);
            var result = _mapper.Map<int>(incidentId);
            var response = new UpdateCompanyIncidentActionResponse();
            if (result > 0)
            {
                response.Data = incidentId;
            }
            else
            {
                response.Data = null;
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<DeleteCompanyIncidentResponse> DeleteCompanyIncident(DeleteCompanyIncidentRequest request)
    {
        try
        {
            var incident = await _incidentRepository.DeleteCompanyIncidents(_currentUser.CompanyId, request.IncidentId, _currentUser.UserId, _currentUser.TimeZone);
            var result = _mapper.Map<bool>(incident);
            var response = new DeleteCompanyIncidentResponse();
            if (result)
            {
                response.IsIncidentDeleted = result;
                response.Message = "Company Incident Deleted";
            }
            else
            {
                response.IsIncidentDeleted = false;
                response.Message = "No data found";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<DeleteCompanyIncidentActionResponse> DeleteCompanyIncidentAction(DeleteCompanyIncidentActionRequest request)
    {
        try
        {
            var incident = await _incidentRepository.DeleteCompanyIncidentActions(_currentUser.CompanyId, request.IncidentId, request.IncidentActionId, _currentUser.UserId, _currentUser.TimeZone);
            var result = _mapper.Map<bool>(incident);
            var response = new DeleteCompanyIncidentActionResponse();
            if (result)
            {
                response.IsIncidentActionDeleted = result;
                response.Message = "Company Incident Deleted";
            }
            else
            {
                response.IsIncidentActionDeleted = false;
                response.Message = "No data found";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<DeleteIncidentAssetResponse> DeleteIncidentAsset(DeleteIncidentAssetRequest request)
    {
        try
        {
            var incident = await _incidentRepository.DeleteIncidentAssets(_currentUser.CompanyId, request.IncidentId, request.AssetObjMapId, request.IncidentAssetId);
            var result = _mapper.Map<bool>(incident);
            var response = new DeleteIncidentAssetResponse();
            if (result)
            {
                response.Result = result;
                response.Message = "Company Incident Deleted";
            }
            else
            {
                response.Result = false;
                response.Message = "No data found";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetActiveIncidentDetailsByIdResponse> GetActiveIncidentDetailsById(GetActiveIncidentDetailsByIdRequest request)
    {
        try
        {
            var incident = await _incidentRepository.ActiveIncidentDetailsById(_currentUser.CompanyId, request.IncidentActivationId, _currentUser.UserId);
            var result = _mapper.Map<UpdateIncidentStatus>(incident);
            var response = new GetActiveIncidentDetailsByIdResponse();
            if (result != null)
            {
                response.Data = result;

            }
            else
            {
                response.Message = "No data found";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<IncidentStatusUpdateResponse> IncidentStatusUpdate(IncidentStatusUpdateRequest request)
    {
        try
        {
            var incident = await _incidentRepository.UpdateIncidentStatus(_currentUser.CompanyId, request.IncidentActivationId, request.Type, _currentUser.TimeZone, _currentUser.UserId,
                        request.UserRole, request.Reason, request.NumberOfKeyHolder, request.CompletionNotes, request.MessageMethod, request.CascadePlanID, false);
            var result = _mapper.Map<UpdateIncidentStatusReturn>(incident);
            var response = new IncidentStatusUpdateResponse();
            if (result != null)
            {
                response.Data = result;
                response.Message = "Data Loaded";
            }
            else
            {
                response.Data = null;
                response.Message = "No data found";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetIncidentLibraryResponse> GetIncidentLibrary(GetIncidentLibraryRequest request)
    {
        try
        {
            var incident = await _incidentRepository.IncidentLibrary(_currentUser.CompanyId);
            var result = _mapper.Map<List<IncidentLibraryDetails>>(incident);
            var response = new GetIncidentLibraryResponse();
            if (result != null)
            {
                response.Data = result;
                response.Message = "Data Loaded";
            }
            else
            {
                response.Data = null;
                response.Message = "No data found";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetIncidentMessageResponse> GetIncidentMessage(GetIncidentMessageRequest request)
    {
        try
        {
            var incident = await _incidentRepository.IncidentMessage(_currentUser.CompanyId, request.IncidentId);
            var result = _mapper.Map<List<ActionLsts>>(incident);
            var response = new GetIncidentMessageResponse();
            if (result != null)
            {
                response.Data = result;
                response.Message = "Data Loaded";
            }
            else
            {
                response.Data = null;
                response.Message = "No data found";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetIncidentActionResponse> GetIncidentAction(GetIncidentActionRequest request)
    {
        try
        {
            var incident = await _incidentRepository.GetIncidentAction(_currentUser.CompanyId, request.IncidentId, request.IncidentActionId);
            var result = _mapper.Map<List<ActionLsts>>(incident);
            var response = new GetIncidentActionResponse();
            if (result != null)
            {
                response.Data = result;
                response.Message = "Data Loaded";
            }
            else
            {
                response.Data = null;
                response.Message = "No data found";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetIncidentAssetResponse> GetIncidentAsset(GetIncidentAssetRequest request)
    {
        try
        {
            var incident = await _incidentRepository.IncidentAsset(_currentUser.CompanyId, request.IncidentId);
            var result = _mapper.Map<List<IncidentAssets>>(incident);
            var response = new GetIncidentAssetResponse();
            if (result != null)
            {
                response.Data = result;
                response.Message = "Data Loaded";
            }
            else
            {
                response.Data = null;
                response.Message = "No data found";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetCompanySOSResponse> GetCompanySOS(GetCompanySOSRequest request)
    {
        try
        {
            var sos = await _incidentRepository.GetCompanySOS(_currentUser.CompanyId);
            var result = _mapper.Map<IncidentDetails>(sos);
            var response = new GetCompanySOSResponse();
            if (result != null)
            {
                response.Data = result;
                response.Message = "Data Loaded";
            }
            else
            {
                response.Data = null;
                response.Message = "Error loading SOS Incident. SOS incident is not configured.";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<UpdateSOSIncidentResponse> UpdateSOSIncident(UpdateSOSIncidentRequest request)
    {
        try
        {
            var IncidentDtl = await _incidentRepository.UpdateCompanyIncidents(_currentUser.CompanyId, request.IncidentId, request.IncidentIcon, request.Name,
                        request.Description, request.PlanAssetID, request.IncidentTypeId, request.Severity, request.Status,
                        request.NumberOfKeyHolders, _currentUser.UserId, _currentUser.TimeZone, request.UpdIncidentKeyHldLst, request.AudioAssetId,
                        request.TrackUser, request.SilentMessage, request.AckOptions, request.MessageMethod, request.CascadePlanID, request.Groups, request.IncidentKeyholder);
            await _incidentRepository.UpdateSOSLocation(request.IncidentId, request.ImpactedLocation);
            await _incidentRepository.UpdateSOSNotificationGroup(request.IncidentId, request.NotificationGroup, _currentUser.CompanyId);
            var result = _mapper.Map<int>(IncidentDtl);
            var response = new UpdateSOSIncidentResponse();
            if (result > 0)
            {
                response.Data = result;
                response.Message = "Data Loaded";
            }
            else
            {
                response.Data = 0;
                response.Message = "No record found.";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<TestWithMeResponse> TestWithMe(TestWithMeRequest request)
    {
        try
        {
            var testMe = await _incidentRepository.TestWithMe(request.IncidentId, request.ImpactedLocationId, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
            var result = _mapper.Map<UpdateIncidentStatusReturn>(testMe);
            var response = new TestWithMeResponse();
            if (result != null)
            {
                response.Data = result;
            }
            else
            {
                response.Data = null;
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetCMDMessageResponse> GetCMDMessage(GetCMDMessageRequest request)
    {
        try
        {
            var testMe = await _incidentRepository.GetCMDMessage(request.IncidentActivationId, _currentUser.UserId);
            var result = _mapper.Map<List<CmdMessage>>(testMe);
            var response = new GetCMDMessageResponse();
            if (result != null)
            {
                response.Data = result;
            }
            else
            {
                response.Data = null;
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetIncidentMapLocationsResponse> GetIncidentMapLocations(GetIncidentMapLocationsRequest request)
    {
        try
        {
            var testMe = await _incidentRepository.GetIncidentMapLocations(request.ActiveIncidentId, request.Filter);
            var result = _mapper.Map<List<MapLocationReturn>>(testMe);
            var response = new GetIncidentMapLocationsResponse();
            if (result != null)
            {
                response.Data = result;
            }
            else
            {
                response.Data = null;
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<SaveIncidentParticipantsResponse> SaveIncidentParticipants(SaveIncidentParticipantsRequest request)
    {
        try
        {
            var testMe = await _incidentRepository.SaveIncidentParticipants(request.IncidentId, request.IncidentActionId, request.IncidentParticipants, request.UsersToNotify);
            var result = _mapper.Map<bool>(testMe);
            var response = new SaveIncidentParticipantsResponse();
            if (result)
            {
                response.Result = result;
                response.Message = "Saved";
            }
            else
            {
                response.Result = false;
                response.Message = "No record found.";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetIncidentByNameResponse> GetIncidentByName(GetIncidentByNameRequest request)
    {
        try
        {
            var testMe = await _incidentRepository.GetIncidentByName(request.IncidentName, _currentUser.CompanyId, _currentUser.UserId, _currentUser.TimeZone);
            var result = _mapper.Map<TourIncident>(testMe);
            var response = new GetIncidentByNameResponse();
            if (result != null)
            {
                response.Result = result;

            }
            else
            {
                response.Result = null;

            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<SaveIncidentJobResponse> SaveIncidentJob(SaveIncidentJobRequest request)
    {
        try
        {
            var testMe = await _incidentRepository.SaveIncidentJob(_currentUser.CompanyId, request.IncidentId, request.IncidentActivationId, request.Description, request.Severity,
                        request.ImpactedLocationId, _currentUser.TimeZone, _currentUser.UserId, request.InitiateIncidentKeyHldLst, request.InitiateIncidentNotificationObjLst,
                        request.MultiResponse, request.AckOptions, request.AudioAssetId, request.TrackUser, request.SilentMessage, request.MessageMethod, request.AffectedLocations,
                        request.UsersToNotify, request.SocialHandle);
            var result = _mapper.Map<bool>(testMe);
            var response = new SaveIncidentJobResponse();
            if (result)
            {
                response.Result = result;
                response.Message = "Incident Job has been added";

            }
            else
            {
                response.Result = false;
                response.Message = "No data found";

            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<SegregationLinksResponse> SegregationLinks(SegregationLinksRequest request)
    {
        try
        {
            var testMe = await _incidentRepository.SegregationLinks(request.TargetID, request.MemberShipType, request.LinkType.ToGrString(), _currentUser.CompanyId);
            var result = _mapper.Map<IncidentSegLinks>(testMe);
            var response = new SegregationLinksResponse();
            if (result != null)
            {
                response.Data = result;
                response.Message = "Links Shared";

            }
            else
            {
                response.Data = null;
                response.Message = "No data found";

            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<UpdateSegregationLinkResponse> UpdateSegregationLink(UpdateSegregationLinkRequest request)
    {
        try
        {
            var links = await _incidentRepository.UpdateSegregationLink(request.SourceID, request.TargetID, request.LinkType.ToGrString(), _currentUser.UserId, _currentUser.CompanyId);
            var result = _mapper.Map<bool>(links);
            var response = new UpdateSegregationLinkResponse();
            if (result)
            {
                response.Result = result;
                response.Message = "Updated";

            }
            else
            {
                response.Result = false;
                response.Message = "No data found";

            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetSOSIncidentResponse> GetSOSIncident(GetSOSIncidentRequest request)
    {
        try
        {
            var incidentDetails = await _incidentRepository.GetCompanySOS(_currentUser.CompanyId);
            var result = _mapper.Map<IncidentDetails>(incidentDetails);
            var response = new GetSOSIncidentResponse();
            if (result != null)
            {
                response.Data = result;
                response.Message = "Data Loaded";

            }
            else
            {
                response.Data = null;
                response.Message = "No data found";

            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetIncidentEntityRecipientResponse> GetIncidentEntityRecipient(GetIncidentEntityRecipientRequest request)
    {
        try
        {
            var dataTable = await _incidentRepository.GetIncidentEntityRecipient(_paging.PageNumber, _paging.PageSize, request.Search, request.Draw, _paging.OrderBy, request.Dir, request.ActiveIncidentID, request.EntityType, request.EntityID, _currentUser.CompanyId, _currentUser.UserId, request.CompanyKey);
            var result = _mapper.Map<DataTablePaging>(dataTable);
            var response = new GetIncidentEntityRecipientResponse();
            if (result != null)
            {
                response.DataTable = result;
                response.Message = "Data Loaded";

            }
            else
            {
                response.DataTable = null;
                response.Message = "No data found";

            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetIncidentRecipientEntityResponse> GetIncidentRecipientEntity(GetIncidentRecipientEntityRequest request)
    {
        try
        {
            var dataTable = await _incidentRepository.GetIncidentRecipientEntity(request.ActiveIncidentID, request.EntityType, _currentUser.UserId, _currentUser.CompanyId);
            var result = _mapper.Map<List<MessageGroupObject>>(dataTable);
            var response = new GetIncidentRecipientEntityResponse();
            if (result != null)
            {
                response.Data = result;
                response.Message = "Data Loaded";

            }
            else
            {
                response.Data = null;
                response.Message = "No data found";

            }
            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

}