using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.GetUserGroups
{
    public class GetUserGroupsValidator:AbstractValidator<GetUserGroupsRequest>
    {
        public GetUserGroupsValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}
