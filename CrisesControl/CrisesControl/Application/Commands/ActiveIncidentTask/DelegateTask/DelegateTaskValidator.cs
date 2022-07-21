using FluentValidation;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.DelegateTask
{
    public class DelegateTaskValidator:AbstractValidator<DelegateTaskRequest>
    {
        public DelegateTaskValidator()
        {
           // RuleFor()
        }
    }
}
