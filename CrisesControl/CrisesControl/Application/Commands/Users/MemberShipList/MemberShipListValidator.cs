using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.MemberShipList
{
    public class MemberShipListValidator: AbstractValidator<MemberShipListRequest>
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
