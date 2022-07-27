using AutoMapper;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Billing.Repositories;
using CrisesControl.Core.Common;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories {
    public class BillingRespository : IBillingRepository {
        private readonly CrisesControlContext _context;
        private readonly IMapper _mapper;

        public BillingRespository(CrisesControlContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BillingPaymentProfile> GetPaymentProfile(int companyID) {
            BillingPaymentProfile BillInfo = new BillingPaymentProfile();

            try {

                var pCompanyId = new SqlParameter("@CompanyID", companyID);
                var Profile = _context.Set<CompanyPaymentProfile>().FromSqlRaw("exec Pro_Company_GetCompanyAccount_PaymentProfile {0}", pCompanyId).ToList().FirstOrDefault();

                if (Profile != null) {
                    BillInfo.Profile = Profile;

                    List<string> stopped_comms = new List<string>();

                    var subscribed_method = _context.Set<CompanySubscribedMethod>().FromSqlRaw("exec Pro_Get_Company_Subscribed_Methods {0}", pCompanyId).ToListAsync()
                        .Result.Select(c => c.MethodCode).ToList();

                    if (subscribed_method.Contains("EMAIL")) {
                        if (Profile.MinimumEmailRate > 0) {
                            stopped_comms.Add("Email");
                        }
                    }
                    if (subscribed_method.Contains("PHONE")) {
                        if (Profile.MinimumPhoneRate > 0) {
                            stopped_comms.Add("Phone");
                        }
                    }
                    if (subscribed_method.Contains("PHONE")) {
                        if (Profile.MinimumPhoneRate > 0) {
                            stopped_comms.Add("Conference");
                        }
                    }
                    if (subscribed_method.Contains("TEXT")) {
                        if (Profile.MinimumTextRate > 0) {
                            stopped_comms.Add("Text");
                        }
                    }
                    if (subscribed_method.Contains("PUSH")) {
                        if (Profile.MinimumPushRate > 0) {
                            stopped_comms.Add("Push");
                        }
                    }
                    if (stopped_comms.Count > 0) {
                        string stopped_service = string.Join(", ", stopped_comms);
                        BillInfo.PaidServices = stopped_service;
                    }
                }
            } catch (Exception ex) {

            }
            return BillInfo;
        }

        public async Task<BillingSummaryModel> GetBillingSummary(int companyId, int userId)
        {
            BillingSummaryModel ResultDTO = new BillingSummaryModel();
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var pUserID = new SqlParameter("@UserID", userId);
                BillingSummaryModel billingInfo = await _context.Set<BillingSummaryModel>().FromSqlRaw("EXEC Pro_Billing_GetBillingSummary @CompanyID, @UserID", pCompanyID, pUserID).FirstOrDefaultAsync()!;

                if (billingInfo != null)
                {
                    return billingInfo;
                }
                else
                {
                    ResultDTO.ErrorId = 110;
                    ResultDTO.Message = "No record found.";
                }
                return ResultDTO;
            }
            catch (Exception ex)
            {
                return ResultDTO;
            }
        }

        public async Task<GetCompanyInvoicesReturn> GetAllInvoices(int CompanyId, CancellationToken cancellationToken)
        {
            GetCompanyInvoicesReturn result = new GetCompanyInvoicesReturn();
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
                var companyInvoices = await _context.Set<CompanyInvoices>().FromSqlRaw("EXEC Pro_Billing_GetAllInvoicesByCompanyRef @CompanyID", pCompanyID).ToListAsync();

                if (companyInvoices.Count > 0)
                {
                    result.AllInvoices = _mapper.Map<List<CompanyInvoices>>(companyInvoices);
                    return result;
                }
                else
                {
                    result.ErrorId = 110;
                    result.Message = "No record found.";
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }

        }

        public async Task<List<InvoiceSchReturn>> GetInvItems(int orderId, int monthVal, int yearVal)
        {
            try
            {
                var pOrderId = new SqlParameter("@OrderId", orderId);
                var pMonthVal = new SqlParameter("@MonthValue", monthVal);
                var pYearVal = new SqlParameter("@YearValue", yearVal);

                var result = await _context.Set<InvoiceSchReturn>().FromSqlRaw("EXEC Billing_Get_Invoice_Schedule @OrderId, @MonthValue, @YearValue",
                    pOrderId, pMonthVal, pYearVal).ToListAsync();
                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new List<InvoiceSchReturn>();
        }

        public async Task<dynamic> GetOrder(int orderId, int companyId, string customerId, int originalOrderId)
        {
            try
            {
                    var pOrderId = new SqlParameter("@OrderId", orderId);
                    var pOriginalOrderId = new SqlParameter("@OriginalOrderId", originalOrderId);
                    var pCompanyId = new SqlParameter("@CompanyId", companyId);
                    var pCustomerId = new SqlParameter("@CustomerId", customerId);

                    var result = await _context.Set<OrderListReturn>().FromSqlRaw("EXEC Billing_Get_Order @OrderId, @OriginalOrderId, @CompanyId, @CustomerId",
                        pOrderId, pOriginalOrderId, pCompanyId, pCustomerId).ToListAsync();

                    if (result != null)
                    {
                        if (orderId > 0 || originalOrderId > 0)
                        {
                            var newresult = result.FirstOrDefault();
                            newresult.Modules = await GetProducts(newresult.OrderID, companyId);
                            newresult.InvItems = await GetInvItems(newresult.OrderID, DateTime.Now.Month, DateTime.Now.Year);

                            return newresult;
                        }
                        return result;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<List<CompanyPackageFeatures>> GetProducts(int OrderId, int CompanyId)
        {
            try
            {
                var pOrderId = new SqlParameter("@OrderId", OrderId);
                var pCompanyId = new SqlParameter("@CompanyId", CompanyId);

                var result = await _context.Set<CompanyPackageFeatures>().FromSqlRaw("EXEC Billing_Get_Order_Items @OrderId, @CompanyId", pOrderId, pCompanyId).ToListAsync();
                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new List<CompanyPackageFeatures>();
        }

        private async Task<int> CreateOrderHeader(int companyId, int orderId, decimal contractValue, int contractDuration, DateTime orderDate,
            string paymentMethod, int keyholderCount, int staffCount, string tigerOrderNo, string invoiceNumber,
            string status, decimal netTotal, decimal vatTotal, DateTime contractStartDate, int currentUserId, string contractType, int activated, decimal discount)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyId", companyId);
                var pOrderId = new SqlParameter("@OrderId", orderId);
                var pContractValue = new SqlParameter("@ContractValue", contractValue);
                var pContractDuration = new SqlParameter("@ContractDuration", contractDuration);
                var pPaymentMethod = new SqlParameter("@PaymentMethod", paymentMethod);
                var pKeyholderCount = new SqlParameter("@KeyholderCount", keyholderCount);
                var pStaffCount = new SqlParameter("@StaffCount", staffCount);
                var pTigerOrderNo = new SqlParameter("@TigerOrderNo", tigerOrderNo);
                var pInvoiceNumber = new SqlParameter("@InvoiceNumber", invoiceNumber);
                var pOrderDate = new SqlParameter("@OrderDate", orderDate);
                var pStatus = new SqlParameter("@Status", status);
                var pNetTotal = new SqlParameter("@NetTotal", netTotal);
                var pVatTotal = new SqlParameter("@VatTotal", vatTotal);
                var pContractStartDate = new SqlParameter("@ContractStartDate", contractStartDate);
                var pCurrentUserId = new SqlParameter("@CurrentUserId", currentUserId);
                var pContractType = new SqlParameter("@ContractType", contractType);
                var pActivated = new SqlParameter("@Activated", activated);
                var pDiscount = new SqlParameter("@Discount", discount);

                var result = await _context.Set<JsonResult>().FromSqlRaw("EXEC Billing_Create_Order @CompanyId, @OrderId, @ContractValue, @ContractDuration, @PaymentMethod, " +
                    "@KeyholderCount, @StaffCount, @TigerOrderNo, @InvoiceNumber, @OrderDate, @Status, @NetTotal, @VatTotal,@ContractStartDate, @CurrentUserId, " +
                    "@ContractType, @Activated, @Discount",
                pCompanyId, pOrderId, pContractValue, pContractDuration, pPaymentMethod, pKeyholderCount, pStaffCount, pTigerOrderNo, pInvoiceNumber, pOrderDate, pStatus,
                pNetTotal, pVatTotal, pContractStartDate, pCurrentUserId, pContractType, pActivated, pDiscount).FirstOrDefaultAsync();
                if (result != null)
                {
                    return result.ResultId;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<int> CreateOrderItem(int orderId, int moduleId, decimal rate, int unit, decimal amount, string added, decimal discount)
        {
            try
            {
                var pOrderID = new SqlParameter("@OrderID", orderId);
                var pModuleID = new SqlParameter("@ModuleID", moduleId);
                var pRate = new SqlParameter("@Rate", rate);
                var pUnit = new SqlParameter("@Unit", unit);
                var pAmount = new SqlParameter("@Amount", amount);
                var pAdded = new SqlParameter("@Added", added);
                var pDiscount = new SqlParameter("@Discount", discount);


                var result = await _context.Set<JsonResult>().FromSqlRaw("EXEC Billing_Create_Order_Item @OrderID, @ModuleID, @Rate, @Unit, @Amount, @Added, @Discount",
                    pOrderID, pModuleID, pRate, pUnit, pAmount, pAdded, pDiscount).FirstOrDefaultAsync();

                if (result != null)
                {
                    return result.ResultId;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> CreateOrder(OrderModel IP)
        {
            int orderId = 0;
            try
            {

                orderId = await CreateOrderHeader(IP.CustomerId, IP.OrderId, IP.ContractValue, IP.ContractDuration, IP.OrderDate,
                IP.PaymentMethod, IP.KeyholderCount, IP.StaffCount, IP.TigerOrderNo, IP.InvoiceNumber, IP.OrderStatus, IP.NetTotal, IP.VatTotal,
                IP.ContractStartDate, IP.CurrentUserId, IP.ContractType, IP.Activated, IP.Discount);

                if (orderId > 0)
                {
                    foreach (Product item in IP.Products)
                    {
                        await CreateOrderItem(orderId, item.ModuleID, item.Rate, item.Unit, item.Amount, item.Added, item.Discount);
                    }

                    //Create Transactions in the transactions table
                    if (IP.ContractStartDate <= DateTime.Now.Date && IP.OrderStatus.ToUpper() == "PAYMENT_COLLECTED" && IP.Activated == 1)
                    {
                        //ProcessOrderTransactions(orderId, IP.CustomerId, IP.CurrentUserId, IP.ContractType);
                        //ProcessPendingOrder(IP.OrderId, IP.CustomerId);
                    }

                    var removefalse = await (from OD in _context.Set<OrderDetail>() where OD.OrderId == orderId && OD.Added == "false" select OD).ToListAsync();
                    if (removefalse != null)
                    {
                        _context.Set<OrderDetail>().RemoveRange(removefalse);
                       await _context.SaveChangesAsync();
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return orderId;
        }

    }
}
