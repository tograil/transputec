using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentUserMessage
{
    public class GetIncidentUserMessageValidator:AbstractValidator<GetIncidentUserMessageRequest>
    {
        public GetIncidentUserMessageValidator()
        {
            RuleFor(x => x.IncidentActivationId).GreaterThan(0);
        }
    }
}
