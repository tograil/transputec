using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Assets.CreateAsset
{
    public class CreateAssetValidator: AbstractValidator<CreateAssetRequest>
    {
        public CreateAssetValidator()
        {
            RuleFor(x => x.AssetId).GreaterThan(0);
            RuleFor(x => x.CompanyId).GreaterThan(0);
        }
    }
}
