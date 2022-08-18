using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Incidents.GetCMDMessage
{
    public class GetCMDMessageValidator:AbstractValidator<GetCMDMessageRequest>
    {
        public GetCMDMessageValidator()
        {
            RuleFor(x => x.IncidentActivationId).GreaterThan(0);
        }
    }
}
