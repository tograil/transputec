using CrisesControl.Api.Application.Commands.Common;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentLocations;

public class GetIncidentLocationsRequest : IRequest<BaseResponse>
{
    public int CompanyId { get; set; }
    public int IncidentActivationId { get; set; }
}