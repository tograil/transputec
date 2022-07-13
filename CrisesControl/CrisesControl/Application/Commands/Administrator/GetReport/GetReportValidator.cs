using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Administrator.GetReport
{
    public class GetReportValidator:AbstractValidator<GetReportRequest>
    {
        public GetReportValidator()
        {
            RuleFor(x => x.ReportId).GreaterThan(0);
        }
    }
}
