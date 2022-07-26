using AutoMapper;
using CrisesControl.Api.Application.Commands.Incidents.AddNotes;
using CrisesControl.Api.Application.Commands.Incidents.CheckUserSOS;
using CrisesControl.Api.Application.Commands.Incidents.GetActiveIncidentBasic;
using CrisesControl.Api.Application.Commands.Incidents.GetAttachments;
using CrisesControl.Api.Application.Commands.Incidents.GetCallToAction;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentSOSRequest;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentTaskNotes;
using CrisesControl.Api.Application.Commands.Incidents.GetIndidentTimeline;
using CrisesControl.Api.Application.Commands.Incidents.UpdateSOS;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query;

public class IncidentQuery : IIncidentQuery
{
    private readonly IActiveIncidentRepository _activeIncidentRepository;
    private readonly IIncidentRepository _incidentRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public IncidentQuery(IActiveIncidentRepository activeIncidentRepository,
        IIncidentRepository incidentRepository,
        ICurrentUser currentUser, IMapper mapper)
    {
        _activeIncidentRepository = activeIncidentRepository;
        _incidentRepository = incidentRepository;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public List<IncidentList> GetAllCompanyIncident(int userId)
    {
        return _incidentRepository.GetCompanyIncident(_currentUser.CompanyId, userId > 0 ? userId : _currentUser.UserId);
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

    public DataTablePaging GetAllActiveCompanyIncident(string? status, DataTableAjaxPostModel pagedRequest)
    {
        var RecordStart = pagedRequest.Start == 0 ? 0 : pagedRequest.Start;
        var RecordLength = pagedRequest.Length == 0 ? int.MaxValue : pagedRequest.Length;
        var SearchString = (pagedRequest.Search != null) ? pagedRequest.Search.Value : "";
        string OrderBy = pagedRequest.Order != null ? pagedRequest.Order.FirstOrDefault().Column : "Name";
        string OrderDir = pagedRequest.Order != null ? pagedRequest.Order.FirstOrDefault().Dir : "asc";
        string Status = status != null ? status : "1,2,3,4";

        int totalRecord = 0;
        DataTablePaging rtn = new DataTablePaging();
        rtn.Draw = pagedRequest.Draw;

        var ActIncidentDtl = _activeIncidentRepository.GetCompanyActiveIncident(_currentUser.CompanyId, _currentUser.UserId, Status, RecordStart, RecordLength, SearchString, OrderBy, OrderDir);

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
        var groups = await _incidentRepository.GetNotes(request.ObjectID, request.NoteType, request.GetAttachments,request.AttachmentType,_currentUser.CompanyId);
        var result = _mapper.Map<List<IncidentTask>>(groups);
        var response = new GetIncidentTaskNotesResponse();
        if (result!=null)
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
        var sos = await _incidentRepository.CheckUserSOS(request.ActiveIncidentId,  _currentUser.UserId);
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
        var basic = await _incidentRepository.GetActiveIncidentBasic( _currentUser.CompanyId, request.IncidentActivationId);
        var result = _mapper.Map<UpdateIncidentStatusReturn>(basic);
        var response = new GetActiveIncidentBasicResponse();
        if (result!=null)
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
            if (result!=null)
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
        catch(Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetCallToActionResponse> GetCallToAction(GetCallToActionRequest request)
    {
        try
        {
            var groups = await _incidentRepository.GetCallToAction(request.ActiveIncidentId,_currentUser.UserId,_currentUser.CompanyId,_currentUser.TimeZone);
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

    public async Task<GetIndidentTimelineResponse> GetIndidentTimeline(GetIndidentTimelineRequest request)
    {
        try
        {
            var groups = await _incidentRepository.GetIndidentTimeline(request.IncidentActivationId,_currentUser.CompanyId,_currentUser.UserId);
            var result = _mapper.Map<List<IncidentMessagesRtn>>(groups);
            var response = new GetIndidentTimelineResponse();
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
}