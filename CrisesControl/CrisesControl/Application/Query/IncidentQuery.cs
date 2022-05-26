﻿using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Application.Query.Common;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Paging;

namespace CrisesControl.Api.Application.Query;

public class IncidentQuery : IIncidentQuery
{
    private readonly IActiveIncidentRepository _activeIncidentRepository;
    private readonly IIncidentRepository _incidentRepository;
    private readonly ICurrentUser _currentUser;

    public IncidentQuery(IActiveIncidentRepository activeIncidentRepository,
        IIncidentRepository incidentRepository,
        ICurrentUser currentUser)
    {
        _activeIncidentRepository = activeIncidentRepository;
        _incidentRepository = incidentRepository;
        _currentUser = currentUser;
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

    public DataTablePaging GetAllActiveCompanyIncident(string? status, PagedRequest pagedRequest)
    {
        var RecordStart = pagedRequest.Start == 0 ? 0 : pagedRequest.Start;
        var RecordLength = pagedRequest.Length == 0 ? int.MaxValue : pagedRequest.Length;
        var SearchString = (pagedRequest.Search != null) ? pagedRequest.Search.value : "";
        string OrderBy = pagedRequest.Order != null ? pagedRequest.Order.FirstOrDefault().column : "Name";
        string OrderDir = pagedRequest.Order != null ? pagedRequest.Order.FirstOrDefault().dir : "asc";
        string Status = status != null ? status : "1,2,3,4";

        int totalRecord = 0;
        DataTablePaging rtn = new DataTablePaging();
        rtn.draw = pagedRequest.Draw;

        var ActIncidentDtl = _activeIncidentRepository.GetCompanyActiveIncident(_currentUser.CompanyId, _currentUser.UserId, Status, RecordStart, RecordLength, SearchString, OrderBy, OrderDir);

        if (ActIncidentDtl != null)
        {
            totalRecord = ActIncidentDtl.Count;
            rtn.recordsFiltered = ActIncidentDtl.Count;
            rtn.data = ActIncidentDtl;
        }

        var TotalList = _activeIncidentRepository.GetCompanyActiveIncident(_currentUser.CompanyId, _currentUser.UserId, Status, 0, int.MaxValue, "", "IncidentActivationId", "asc");

        if (TotalList != null)
        {
            totalRecord = TotalList.Count;
        }

        rtn.recordsTotal = totalRecord;

        return rtn;
    }

}