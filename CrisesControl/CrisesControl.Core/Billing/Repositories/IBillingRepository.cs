using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Billing.Repositories {
    public interface IBillingRepository {
        Task<BillingPaymentProfile> GetPaymentProfile(int companyID);
        Task<BillingSummaryModel> GetBillingSummary(int outUserCompanyId, int chkUserId);
        Task<GetCompanyInvoicesReturn> GetAllInvoices(int companyId, CancellationToken cancellationToken);
        Task<List<InvoiceSchReturn>> GetInvItems(int OrderId, int MonthVal, int YearVal);
        Task<dynamic> GetOrder(int orderId, int companyId, string customerId, int originalOrderId);
    }
}
