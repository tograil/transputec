using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.TakeOwnership
{
    public class TakeOwnershipRequest:IRequest<TakeOwnershipResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
    }
}
