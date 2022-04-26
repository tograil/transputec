using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Paging;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAllActiveCompanyIncident;

public class GetAllActiveCompanyIncidentHandler 
    : IRequestHandler<GetAllActiveCompanyIncidentRequest, GetAllActiveCompanyIncidentResponse>
{
    private readonly ICurrentUser _currentUser;
    private readonly IActiveIncidentRepository _activeIncidentRepository;

    public GetAllActiveCompanyIncidentHandler(ICurrentUser currentUser,
        ICompanyRepository companyRepository,
        IActiveIncidentRepository activeIncidentRepository)
    {
        _currentUser = currentUser;
        _activeIncidentRepository = activeIncidentRepository;
    }

    public async Task<GetAllActiveCompanyIncidentResponse> Handle(GetAllActiveCompanyIncidentRequest request, CancellationToken cancellationToken)
    {
        var RecordStart = request.start == 0 ? 0 : request.start;
        var RecordLength = request.length == 0 ? int.MaxValue : request.length;
        var SearchString = (request.search != null) ? request.search.value : "";
        string OrderBy = request.order != null ? request.order.FirstOrDefault().column : "Name";
        string OrderDir = request.order != null ? request.order.FirstOrDefault().dir : "asc";
        string Status = request.Status != null ? request.Status : "1,2,3,4";

        int totalRecord = 0;
        DataTablePaging rtn = new DataTablePaging();
        rtn.draw = request.draw;

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

        if (rtn.data != null)
        {
            return new GetAllActiveCompanyIncidentResponse()
            {
                Data = rtn,
                ErrorCode = "0"
            };
        }
        else
        {
            return new GetAllActiveCompanyIncidentResponse()
            {
                Data = rtn,
                ErrorCode = "110"
            };
        }
    }
}