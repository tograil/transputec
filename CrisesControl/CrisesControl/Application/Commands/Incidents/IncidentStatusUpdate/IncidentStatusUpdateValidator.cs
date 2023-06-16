using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Incidents.IncidentStatusUpdate
{
    public class IncidentStatusUpdateValidator:AbstractValidator<IncidentStatusUpdateRequest>
    {
        public IncidentStatusUpdateValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.Type));
        }
    }
}
