using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.OffDutyReport
{
    public class OffDutyReportValidator:AbstractValidator<OffDutyReportRequest>
    {
        public OffDutyReportValidator()
        {
            RuleFor(x => x.CompanyId).GreaterThan(0);
        }
    }
}
