using FluentValidation;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.SendTaskUpdate
{
    public class SendTaskUpdateValidator:AbstractValidator<SendTaskUpdateRequest>
    {
        public SendTaskUpdateValidator()
        {
            //RuleFor();
        }
    }
}
