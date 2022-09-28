using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.ResponsesSummary
{
    public class ResponseSummaryValidator: AbstractValidator<ResponseSummaryRequest>
    {
        public ResponseSummaryValidator()
        {
            RuleFor(x=>x.MessageId).GreaterThan(0);
        }
    }
}
