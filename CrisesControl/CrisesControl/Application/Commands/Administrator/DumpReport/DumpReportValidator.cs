using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Administrator.DumpReport
{
    public class DumpReportValidator:AbstractValidator<DumpReportRequest>
    {
        public DumpReportValidator()
        {
            RuleFor(x => x.ReportID).GreaterThan(0);
        }
    }
}
