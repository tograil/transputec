using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Assets.DeleteAsset
{
    public class DeleteAssetValidator:AbstractValidator<DeleteAssetRequest>
    {
        public DeleteAssetValidator()
        {
            RuleFor(x => x.AssetId).GreaterThan(0);
        }
    }
}
