using CrisesControl.Api.Application.Commands.Common;
using CrisesControl.Core.Incidents.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetCompanyIncidentType;

public class GetCompanyIncidentTypeHandler 
    : IRequestHandler<GetCompanyIncidentTypeRequest, BaseResponse>
{
    private readonly IIncidentRepository _incidentRepository;

    public GetCompanyIncidentTypeHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<BaseResponse> Handle(GetCompanyIncidentTypeRequest request, CancellationToken cancellationToken)
    {
        var IncidentTypes = await _incidentRepository.CompanyIncidentType(request.CompanyId);

        if (IncidentTypes != null)
        {
            return new BaseResponse()
            {
                Data = IncidentTypes,
                ErrorCode = "0"
            };
        }
        else
        {
            return new BaseResponse()
            {
                Data = IncidentTypes,
                ErrorCode = "110",
                Message = "No record found."
            };
        }
    }
}