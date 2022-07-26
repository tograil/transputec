using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.ExportAcknowledgement
{
    public class ExportAcknowledgementValidator:AbstractValidator<ExportAcknowledgementRequest>
    {
        public ExportAcknowledgementValidator()
        {
            RuleFor(x => x.MessageId).GreaterThan(0);
        }
    }
}
