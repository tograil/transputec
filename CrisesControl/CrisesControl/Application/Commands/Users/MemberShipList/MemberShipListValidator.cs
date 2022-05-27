using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.MembershipList
{
    public class MemberShipListValidator: AbstractValidator<MembershipRequest>
    {
        public MemberShipListValidator()
        {
            RuleFor(x => x.TargetID)
                       .GreaterThan(0);
            RuleFor(x => x.ObjMapID)
                .GreaterThan(0);
        }
    }
}
