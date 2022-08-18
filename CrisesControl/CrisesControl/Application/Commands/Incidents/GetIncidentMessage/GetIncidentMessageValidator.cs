using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentMessage
{
    public class GetIncidentMessageValidator:AbstractValidator<GetIncidentMessageRequest>
    {
        public GetIncidentMessageValidator()
        {
            RuleFor(x => x.IncidentId).GreaterThan(0);
        }
    }
}
