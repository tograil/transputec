using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Assets.CreateAsset
{
    public class CreateAssetValidator: AbstractValidator<CreateAssetRequest>
    {
        public CreateAssetValidator()
        {
            
            RuleFor(x => x.CompanyId).GreaterThan(0);
        }
    }
}
