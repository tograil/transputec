using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentMapLocations
{
    public class GetIncidentMapLocationsRequest:IRequest<GetIncidentMapLocationsResponse>
    {
        public int ActiveIncidentId { get; set; }
        //public string Filter { get; set; }
    }
}
