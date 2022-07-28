using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Billing.Repositories {
    public interface IBillingRepository {
        Task<BillingPaymentProfile> GetPaymentProfile(int companyID);
        Task<BillingSummaryModel> GetBillingSummary(int outUserCompanyId, int chkUserId);
        Task<GetCompanyInvoicesReturn> GetAllInvoices(int companyId, CancellationToken cancellationToken);
        Task<dynamic> GetUnbilledSummary(int startYear);
        Task<dynamic> GetUnbilledSummaryByMonth(int startYear, int monthNumber);
        Task<dynamic> GetUnbilledSummaryByMessage(int messageId);
    }
}
