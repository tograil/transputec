using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.GetLocations
{
    public class GetLocationsRequest : IRequest<GetLocationsResponse>
    {
        public int CompanyId { get; set; }
        public bool FilterVirtual { get; set; }
    }
}
