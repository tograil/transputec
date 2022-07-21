using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.OffDutySetting
{
    public class OffDutySettingValidator : AbstractValidator<OffDutySettingRequest>
    {
        public OffDutySettingValidator()
        {
            RuleFor(x => x.OffDutyAction)
                .NotEmpty();
            RuleFor(x => x.StartDateTime)
                .NotEmpty();
        }
    }
}
