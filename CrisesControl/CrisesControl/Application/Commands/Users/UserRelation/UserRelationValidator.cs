using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.UserRelation
{
    public class UserRelationValidator : AbstractValidator<UserRelationRequest>
    {
        public UserRelationValidator()
        {
        }
    }
}
