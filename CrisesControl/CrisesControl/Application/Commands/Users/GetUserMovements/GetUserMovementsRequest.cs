using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserMovements
{
    public class GetUserMovementsRequest : IRequest<GetUserMovementsResponse>
    {
        public int UserId { get; set; }
    }
}
