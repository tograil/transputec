using CrisesControl.Api.Application.Commands.Payments.AddRemoveModule;
using CrisesControl.Api.Application.Commands.Payments.GetCompanyPackageItems;
using CrisesControl.Api.Application.Commands.Payments.GetPackageAddons;
using CrisesControl.Api.Application.Commands.Payments.UpdateCompanyPaymentProfile;
using CrisesControl.Api.Application.Commands.Payments.UpgradeByKey;
using CrisesControl.Api.Application.Commands.Payments.UpgradePackage;

namespace CrisesControl.Api.Application.Query
{
    public interface IPaymentQuery
    {
        Task<UpgradeByKeyResponse> UpgradeByKey(UpgradeByKeyRequest request);
        Task<UpdateCompanyPaymentProfileResponse> UpdateCompanyPaymentProfile(UpdateCompanyPaymentProfileRequest request);
        Task<GetCompanyPackageItemsResponse> GetCompanyPackageItems(GetCompanyPackageItemsRequest request);
        Task<GetPackageAddonsResponse> GetPackageAddons(GetPackageAddonsRequest request);
        Task<UpgradePackageResponse> UpgradePackage(UpgradePackageRequest request);
        Task<AddRemoveModuleResponse> AddRemoveModule(AddRemoveModuleRequest request);
    }
}
