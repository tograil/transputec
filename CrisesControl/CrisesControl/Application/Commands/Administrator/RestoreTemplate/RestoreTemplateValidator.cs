using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Administrator.RestoreTemplate
{
    public class RestoreTemplateValidator:AbstractValidator<RestoreTemplateRequest>
    {
        public RestoreTemplateValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.Code));
        }
    }
}
