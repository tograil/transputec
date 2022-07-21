using FluentValidation;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveCheckListResponse
{
    public class SaveActiveCheckListResponseValidator:AbstractValidator<SaveActiveCheckListResponseRequest>
    {
        public SaveActiveCheckListResponseValidator()
        {
            RuleFor(x => x.ActiveIncidentTaskID).GreaterThan(0);
        }
    }
}
