using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Administrator.GetEmailTemplate
{
    public class GetEmailTemplateValidator:AbstractValidator<GetEmailTemplateRequest>
    {
        public GetEmailTemplateValidator()
        {
            RuleFor(x => x.TemplateID).GreaterThan(0);
            RuleFor(x => x.QCompanyID).GreaterThan(0);
        }
    }
}
