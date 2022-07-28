using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.IncidentResponseDump
{
    public class IncidentResponseDumpValidator:AbstractValidator<IncidentResponseDumpRequest>
    {
        public IncidentResponseDumpValidator()
        {
            RuleFor(x => x.IncidentActivationId).GreaterThan(0);
        }
    }
}
