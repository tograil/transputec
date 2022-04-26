using CrisesControl.Api.Application.Commands.Common;
using CrisesControl.Core.Incidents.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAffectedLocations;

public class GetAffectedLocationsHandler
    : IRequestHandler<GetAffectedLocationsRequest, BaseResponse>
{
    private readonly IIncidentRepository _incidentRepository;

    public GetAffectedLocationsHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<BaseResponse> Handle(GetAffectedLocationsRequest request, CancellationToken cancellationToken)
    {
        var locs = _incidentRepository.GetAffectedLocation(request.CompanyId, request.LocationType);

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