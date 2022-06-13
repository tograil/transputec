using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetMessageDeliverySummary
{
    public class GetMessageDeliverySummaryValidator : AbstractValidator<GetMessageDeliverySummaryRequest>
    {
        public GetMessageDeliverySummaryValidator()
        {
            RuleFor(x => x.MessageID).GreaterThan(0).ToString();
        }
    }
}
