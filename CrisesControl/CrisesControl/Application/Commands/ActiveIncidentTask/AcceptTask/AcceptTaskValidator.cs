using FluentValidation;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.AcceptTask
{
    public class AcceptTaskValidator:AbstractValidator<AcceptTaskRequest>
    {
        public AcceptTaskValidator()
        {
            RuleFor(x => x.ActiveIncidentTaskID).GreaterThan(0);
        }
    }
}
