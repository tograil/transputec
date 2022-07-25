using FluentValidation;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.ReallocateTask
{
    public class ReallocateTaskValidator:AbstractValidator<ReallocateTaskRequest>
    {
        public ReallocateTaskValidator()
        {

        }
    }
}
