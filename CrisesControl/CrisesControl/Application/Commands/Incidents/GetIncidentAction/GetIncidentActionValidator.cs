using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentAction
{
    public class GetIncidentActionValidator : AbstractValidator<GetIncidentActionRequest>
    {
        public GetIncidentActionValidator()
        {
            RuleFor(x => x.IncidentActionId).GreaterThan(0);
            RuleFor(x => x.IncidentId).GreaterThan(0);
        }
    }
}
