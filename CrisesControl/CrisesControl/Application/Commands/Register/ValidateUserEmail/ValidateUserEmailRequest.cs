using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.ValidateUserEmail
{
    public class ValidateUserEmailRequest :IRequest<ValidateUserEmailResponse>
    {
        public string uniqueId { get; set; }
        public int CompanyId { get; set; }
    }
}
