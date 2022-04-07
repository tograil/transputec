using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Groups.GetGroup
{
    public class GetGroupValidator: AbstractValidator<GetGroupRequest>
    {
        public GetGroupValidator()
        {
            RuleFor(x => x.GroupId)
                .GreaterThan(0);
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
