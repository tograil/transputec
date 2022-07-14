using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Billing
{
    public class GetCompanyInvoicesReturn
    {
        public List<CompanyInvoices> AllInvoices { get; set; }
        public int ErrorId { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
    }

    public class CompanyInvoices
    {
        public int HeaderId { get; set; }
        public DateTimeOffset StatementDate { get; set; }
        public decimal TotalInvoiceAmount { get; set; }
        public decimal NetTotal { get; set; }
        public decimal VATValue { get; set; }
        public DateTimeOffset BillingStartDate { get; set; }
        public DateTimeOffset BillingEndDate { get; set; }
    }
}
