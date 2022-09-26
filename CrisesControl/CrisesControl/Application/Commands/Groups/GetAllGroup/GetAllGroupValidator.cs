using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Groups.GetAllGroup
{
    public class GetAllGroupValidator: AbstractValidator<GetAllGroupRequest>
    {
        public GetAllGroupValidator()
        {
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
