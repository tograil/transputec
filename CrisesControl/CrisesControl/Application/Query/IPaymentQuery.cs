using CrisesControl.Api.Application.Commands.Payments.UpgradeByKey;

namespace CrisesControl.Api.Application.Query
{
    public interface IPaymentQuery
    {
        Task<UpgradeByKeyResponse> UpgradeByKey(UpgradeByKeyRequest request);
    }
}
