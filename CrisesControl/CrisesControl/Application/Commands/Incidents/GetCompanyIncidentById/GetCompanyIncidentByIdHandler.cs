using CrisesControl.Api.Application.Commands.Common;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Incidents.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetCompanyIncidentById;

public class GetCompanyIncidentByIdHandler
    : IRequestHandler<GetCompanyIncidentByIdRequest, BaseResponse>
{
    private readonly ICurrentUser _currentUser;
    private readonly IIncidentRepository _incidentRepository;

    public GetCompanyIncidentByIdHandler(IIncidentRepository incidentRepository, ICurrentUser currentUser)
    {
        _currentUser = currentUser;
        _incidentRepository = incidentRepository;
    }

    public async Task<BaseResponse> Handle(GetCompanyIncidentByIdRequest request, CancellationToken cancellationToken)
    {
        var IncidentDtl = _incidentRepository.GetIncidentById(request.CompanyId, _currentUser.UserId, request.IncidentId, request.UserStatus);

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
        };
    }
}