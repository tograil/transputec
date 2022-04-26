using CrisesControl.Api.Application.Commands.Common;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Incidents.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAllCompanyIncident;

public class GetAllCompanyIncidentHandler 
    : IRequestHandler<GetAllCompanyIncidentRequest, BaseResponse>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly ICurrentUser _currentUser;

    public GetAllCompanyIncidentHandler(IIncidentRepository incidentRepository,
        ICurrentUser currentUser)
    {
        _incidentRepository = incidentRepository;
        _currentUser = currentUser;
    }

    public async Task<BaseResponse> Handle(GetAllCompanyIncidentRequest request, CancellationToken cancellationToken)
    {
        var IncidentDtl = _incidentRepository.GetCompanyIncident(_currentUser.CompanyId, request.QUserId > 0 ? request.QUserId : _currentUser.UserId);

        if (IncidentDtl != null)
        {
            return new BaseResponse()
            {
                Data = IncidentDtl,
                ErrorCode = "0"
            };
        }
        else
        {
            return new BaseResponse()
            {
                Data = IncidentDtl,
                ErrorCode = "110",
                Message = "No record found."
            };
        }
    }
}