using CrisesControl.Api.Application.Commands.Common;
using CrisesControl.Core.Incidents.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentLocations;

public class GetIncidentLocationsHandler
    : IRequestHandler<GetIncidentLocationsRequest, BaseResponse>
{
    private readonly IIncidentRepository _incidentRepository;

    public GetIncidentLocationsHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<BaseResponse> Handle(GetIncidentLocationsRequest request, CancellationToken cancellationToken)
    {
        var locs = _incidentRepository.GetIncidentLocation(request.CompanyId, request.IncidentActivationId);

        if (locs != null)
        {
            return new BaseResponse()
            {
                Data = locs,
                ErrorCode = "0"
            };
        }
        else
        {
            return new BaseResponse()
            {
                Data = locs,
                ErrorCode = "110",
                Message = "No record found."
            };
        };
    }
}