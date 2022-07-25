using FluentValidation;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.TakeOwnership
{
    public class TakeOwnershipValidator:AbstractValidator<TakeOwnershipRequest>
    {
        public TakeOwnershipValidator()
        {
            RuleFor(x => x.ActiveIncidentTaskID).GreaterThan(0);
        }
    }
}
