using CrisesControl.Api.Application.Commands.Common;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAffectedLocations;

public class GetAffectedLocationsRequest : IRequest<BaseResponse>
{
    public int CompanyId { get; set; }
    public string? LocationType { get; set; }
}