using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Administrator.GetEmailFields
{
    public class GetEmailFieldsValidator:AbstractValidator<GetEmailFieldsRequest>
    {
        public GetEmailFieldsValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.TemplateCode));
            RuleFor(x => x.FieldType).GreaterThan(0);
        }
    }
}
