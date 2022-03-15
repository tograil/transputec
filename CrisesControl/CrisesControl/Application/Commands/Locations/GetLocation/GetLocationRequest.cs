using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.GetLocation
{
    public class GetLocationRequest : IRequest<GetLocationResponse>
    {
        public int LocationId { get; set; }  
    }
}
