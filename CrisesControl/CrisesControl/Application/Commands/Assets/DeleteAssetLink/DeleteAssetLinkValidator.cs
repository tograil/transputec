using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Assets.DeleteAssetLink
{
    public class DeleteAssetLinkValidator:AbstractValidator<DeleteAssetLinkRequest>
    {
        public DeleteAssetLinkValidator()
        {
            RuleFor(x => x.AssetID).GreaterThan(0);
        }
    }
}
