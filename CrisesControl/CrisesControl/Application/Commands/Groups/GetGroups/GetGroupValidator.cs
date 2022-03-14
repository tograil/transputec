using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Groups.GetGroups
{
    public class GetGroupsValidator: AbstractValidator<GetGroupsRequest>
    {
        public GetGroupsValidator()
        {
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
