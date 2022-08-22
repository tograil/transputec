using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.GetGroupUsers
{
    public class GetGroupUsersValidator:AbstractValidator<GetGroupUsersRequest>
    {
        public GetGroupUsersValidator()
        {
            RuleFor(x => x.GroupId).GreaterThan(0);
            RuleFor(x => x.ObjectMappingId).GreaterThan(0);
        }
    }
}
