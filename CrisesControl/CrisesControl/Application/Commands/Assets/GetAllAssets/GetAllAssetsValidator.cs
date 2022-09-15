using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Assets.GetAllAssets
{
    public class GetAllAssetsValidator:AbstractValidator<GetAllAssetsRequest>
    {
        public GetAllAssetsValidator()
        {
            RuleFor(x => x.Draw).GreaterThan(0);
        }
    }
}
