using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Assets.GetAssetLink
{
    public class GetAssetLinkValidator:AbstractValidator<GetAssetLinkRequest>
    {
        public GetAssetLinkValidator()
        {
            RuleFor(x => x.AssestID).GreaterThan(0);
        }
    }
}
