using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAffectedLocations
{
    public class GetAffectedLocationsRequest:IRequest<GetAffectedLocationsResponse>
    {
         public LocationType LocationType { get; set; }
    }
}
