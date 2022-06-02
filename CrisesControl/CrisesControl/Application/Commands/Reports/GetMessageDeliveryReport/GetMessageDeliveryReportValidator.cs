using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetMessageDeliveryReport
{
    public class GetMessageDeliveryReportValidator : AbstractValidator<GetMessageDeliveryReportRequest>
    {
        public GetMessageDeliveryReportValidator()
        {
            RuleFor(x => x.StartDate).LessThanOrEqualTo(DateTimeOffset.UtcNow);
            RuleFor(x => x.EndDate).GreaterThan(x=>x.StartDate);
        }
    }
}
