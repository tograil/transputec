using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.DeleteLocation
{
    public class DeleteLocationRequest:IRequest<DeleteLocationResponse>
    {
        public int LocationId { get; set; }
    }
}
