using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageItems
{
    public class GetCompanyPackageItemsValidator:AbstractValidator<GetCompanyPackageItemsRequest>
    {
        public GetCompanyPackageItemsValidator()
        {
            RuleFor(x => x.PackageItemId).GreaterThan(0);
        }
    }
}
