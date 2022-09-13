using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserId
{
    public class GetUserIdRequest : IRequest<GetUserIdResponse>
    {
        public int CompanyId { get; set; }
        public string EmailAddress { get; set; }
    }
}
