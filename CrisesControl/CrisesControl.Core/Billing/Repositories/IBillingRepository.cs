﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Billing.Repositories {
    public interface IBillingRepository {
        Task<BillingPaymentProfile> GetPaymentProfile(int companyID);
        Task<BillingSummaryModel> GetBillingSummary(int outUserCompanyId, int chkUserId);
        Task<GetCompanyInvoicesReturn> GetAllInvoices(int companyId);
        Task<dynamic> GetUnbilledSummary(int startYear);
        Task<dynamic> GetUnbilledSummaryByMonth(int startYear, int monthNumber);
        Task<dynamic> GetUnbilledSummaryByMessage(int messageId);
        Task<List<InvoiceSchReturn>> GetInvItems(int OrderId, int MonthVal, int YearVal);
        Task<List<OrderListReturn>> GetOrders(int orderId, int companyId);
        Task<int> CreateOrder(OrderModel IP);
        Task AddUserRoleChange(int CompanyID, int UserID, string UserRole, string TimeZoneId);
        Task<int> CreateInvoiceSchedule(OrderModel orderModel);
        Task<GetInvoiceByIdResponse> GetInvoicesById(int companyId, int transactionHeaderId, bool showPayments);
        Task<List<TransactionItemDetails>> GetTransactionItem(int companyId, int messageId, string method, int recordStart = 0, int recordLength = 100, string searchString = "", string orderBy = "Name", string orderDir = "asc", string companyKey = "");
        Task<List<UsageGraph>> GetUsageGraph(int companyId, string reportType, int lastMonth);
        Task<int> UpdateCompanyTranscationType(int companyId, int currntUserId, string timeZoneId, int transactionTypeId, decimal transactionRate,
           int compnayTranscationTypeId = 0, string paymentPeriod = "MONTHLY", DateTimeOffset? nextRunDate = null, string paymentMethod = "INVOICE");
    }
}
