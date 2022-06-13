using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.CheckEmail
{
    public class CheckEmailRequest : IRequest<CheckEmailResponse>
    {
        public string EmailId { get; set; }
    }
}
