using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetPingReportChart
{
    public class GetPingReportChartValidator:AbstractValidator<GetPingReportChartRequest> 
    {
        public GetPingReportChartValidator()
        {
            RuleFor(x => x.StartDate).LessThanOrEqualTo(DateTimeOffset.Now);
            
        }
    }
}
