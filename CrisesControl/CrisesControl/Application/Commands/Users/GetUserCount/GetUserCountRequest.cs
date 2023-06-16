using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserCount
{
    public class GetUserCountRequest : IRequest<GetUserCountResponse>
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
    }
}
