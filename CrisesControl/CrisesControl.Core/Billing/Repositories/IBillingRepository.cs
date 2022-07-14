using System.Threading.Tasks;

namespace CrisesControl.Core.Billing.Repositories {
    public interface IBillingRepository {
        Task<BillingPaymentProfile> GetPaymentProfile(int companyID);
        Task<BillingSummaryModel> GetBillingSummary(int outUserCompanyId, int chkUserId);
    }
}
