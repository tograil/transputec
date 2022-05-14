using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Assets.GetAssets
{
    public class GetAssetsValidator: AbstractValidator<GetAssetsRequest>
    {
        public GetAssetsValidator()
        {
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);
        }
    }
}
