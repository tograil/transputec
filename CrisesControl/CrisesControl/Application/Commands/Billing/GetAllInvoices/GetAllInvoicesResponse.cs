﻿using CrisesControl.Api.Application.Commands.Shared;
using CrisesControl.Core.Billing;

namespace CrisesControl.Api.Application.Commands.Billing.GetAllInvoices
{
    public class GetAllInvoicesResponse : CommonModel
    {
        public GetCompanyInvoicesReturn AllInvoices { get; set; }
    }

    public class AllInvoices
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
