using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetPingReportAnalysis
{
    public class GetPingReportAnalysisValidator:AbstractValidator<GetPingReportAnalysisRequest>
    {
        public GetPingReportAnalysisValidator()
        {
            RuleFor(x => x.MessageId).GreaterThan(0);
        }
    }
}
