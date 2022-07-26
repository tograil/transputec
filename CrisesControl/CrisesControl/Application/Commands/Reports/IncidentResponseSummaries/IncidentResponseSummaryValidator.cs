using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.IncidentResponseSummary
{
    public class IncidentResponseSummaryValidator:AbstractValidator<IncidentResponseSummaryRequest>
    {
        public IncidentResponseSummaryValidator()
        {
            RuleFor(x => x.ActiveIncidentID).GreaterThan(0);
        }
    }
}
