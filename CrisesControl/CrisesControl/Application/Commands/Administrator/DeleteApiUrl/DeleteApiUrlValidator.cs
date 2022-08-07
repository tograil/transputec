using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Administrator.DeleteApiUrl
{
    public class DeleteApiUrlValidator:AbstractValidator<DeleteApiUrlRequest>
    {
        public DeleteApiUrlValidator()
        {
            RuleFor(x => x.ApiID).GreaterThan(0);
        }
    }
}
