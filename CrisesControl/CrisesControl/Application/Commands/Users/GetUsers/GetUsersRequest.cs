using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUsers
{
    public class GetUsersRequest : IRequest<GetUsersResponse>
    {
        public int CompanyId { get; set; }
    }
}
