using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Register.DeleteTempRegistration
{
    public class DeleteTempRegistrationValidator:AbstractValidator<DeleteTempRegistrationRequest>
    {
        public DeleteTempRegistrationValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.UniqueReference));
        }
    }
}
