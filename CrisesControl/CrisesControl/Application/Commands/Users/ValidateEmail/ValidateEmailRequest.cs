using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.ValidateEmail
{
    public class ValidateEmailRequest: IRequest<ValidateEmailResponse>
    {
        public string UserEmail { get; set; }
    }
}
