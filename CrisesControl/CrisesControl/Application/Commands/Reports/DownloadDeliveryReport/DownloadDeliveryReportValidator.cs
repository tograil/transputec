using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.DownloadDeliveryReport
{
    public class DownloadDeliveryReportValidator:AbstractValidator<DownloadDeliveryReportRequest>
    {
        public DownloadDeliveryReportValidator()
        {
            RuleFor(x => x.MessageID).GreaterThan(0);
        }
    }
}
