using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Billing.Repositories;
using CrisesControl.Core.Common;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Import;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class BillingRespository : IBillingRepository
    {
        private readonly CrisesControlContext _context;
        private readonly IMapper _mapper;
        private readonly UsageHelper _usage;
        private readonly DBCommon _DBC;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int companyId;

        public BillingRespository(CrisesControlContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _mapper = mapper;            
            _httpContextAccessor = httpContextAccessor;
            _DBC = new DBCommon(_context,_httpContextAccessor);
            _usage = new UsageHelper(_context);
            companyId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
            _DBC = new DBCommon(context, _httpContextAccessor);
            _usage = new UsageHelper(context);
        }

        public async Task<BillingPaymentProfile> GetPaymentProfile(int companyID)
        {
            BillingPaymentProfile BillInfo = new BillingPaymentProfile();

            try
            {

                var pCompanyId = new SqlParameter("@CompanyID", companyID);
                var Profile = _context.Set<CompanyPaymentProfile>().FromSqlRaw("exec Pro_Company_GetCompanyAccount_PaymentProfile {0}", pCompanyId).ToList().FirstOrDefault();

                if (Profile != null)
                {
                    BillInfo.Profile = Profile;

                    List<string> stopped_comms = new List<string>();

                    var subscribed_method = _context.Set<CompanySubscribedMethod>().FromSqlRaw("exec Pro_Get_Company_Subscribed_Methods {0}", pCompanyId).ToListAsync()
                        .Result.Select(c => c.MethodCode).ToList();

                    if (subscribed_method.Contains("EMAIL"))
                    {
                        if (Profile.MinimumEmailRate > 0)
                        {
                            stopped_comms.Add("Email");
                        }
                    }
                    if (subscribed_method.Contains("PHONE"))
                    {
                        if (Profile.MinimumPhoneRate > 0)
                        {
                            stopped_comms.Add("Phone");
                        }
                    }
                    if (subscribed_method.Contains("PHONE"))
                    {
                        if (Profile.MinimumPhoneRate > 0)
                        {
                            stopped_comms.Add("Conference");
                        }
                    }
                    if (subscribed_method.Contains("TEXT"))
                    {
                        if (Profile.MinimumTextRate > 0)
                        {
                            stopped_comms.Add("Text");
                        }
                    }
                    if (subscribed_method.Contains("PUSH"))
                    {
                        if (Profile.MinimumPushRate > 0)
                        {
                            stopped_comms.Add("Push");
                        }
                    }
                    if (stopped_comms.Count > 0)
                    {
                        string stopped_service = string.Join(", ", stopped_comms);
                        BillInfo.PaidServices = stopped_service;
                    }
                }
            }
            catch (Exception ex)
            {

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

        public async Task<GetCompanyInvoicesReturn> GetAllInvoices(int companyId)
        {
            GetCompanyInvoicesReturn result = new GetCompanyInvoicesReturn();
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", companyId);
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

        public async Task<List<OrderListReturn>> GetOrders(int orderId, int companyId)
        {
            try
            {
                var pOrderId = new SqlParameter("@OrderId", orderId);
               
                var pCompanyId = new SqlParameter("@CompanyId", companyId);
               

                var result = await _context.Set<OrderListReturn>().FromSqlRaw("EXEC Billing_Get_Order @OrderId, @CompanyId",
                    pOrderId, pCompanyId).ToListAsync();

                if (result != null)
                {
                    if (orderId > 0 )
                    {
                        var newresult = result.FirstOrDefault();
                        newresult.Modules = await GetProducts(newresult.OrderID, companyId);
                        newresult.InvItems = await GetInvItems(newresult.OrderID, DateTime.Now.Month, DateTime.Now.Year);
                        var r = new List<OrderListReturn>();
                        r.Add(newresult);
                        return r;
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

        private async Task<int> CreateInvoiceScheduleOrderHeader(int companyId, string customerId, int orderId, double contractValue, int contractDuration, DateTime orderDate,
            string paymentMethod, int keyholderCount, int staffCount, string tigerOrderNo, string invoiceNumber,
            string status, double netTotal, double vatTotal, DateTime contractStartDate)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyId", companyId);
                var pCustomerId = new SqlParameter("@CustomerId", customerId);
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

                var result = await _context.Set<JsonResult>().FromSqlRaw("EXEC Billing_Create_Order @CompanyId, @CustomerId, @OrderId, @ContractValue, @ContractDuration, @PaymentMethod, " +
                    "@KeyholderCount, @StaffCount, @TigerOrderNo, @InvoiceNumber, @OrderDate, @Status, @NetTotal, @VatTotal,@ContractStartDate",
                pCompanyId, pCustomerId, pOrderId, pContractValue, pContractDuration, pPaymentMethod, pKeyholderCount, pStaffCount, pTigerOrderNo, pInvoiceNumber, pOrderDate, pStatus,
                pNetTotal, pVatTotal, pContractStartDate).FirstOrDefaultAsync();
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

        private async Task<int> CreateInvoiceScheduleOrderItem(int orderId, string itemName, double rate, int unit, double amount, string chargeType)
        {
            try
            {
                var pOrderID = new SqlParameter("@OrderID", orderId);
                var pItemName = new SqlParameter("@ItemName", itemName);
                var pRate = new SqlParameter("@Rate", rate);
                var pUnit = new SqlParameter("@Unit", unit);
                var pAmount = new SqlParameter("@Amount", amount);
                var pChargeType = new SqlParameter("@ChargeType", chargeType);


                var result = await _context.Set<JsonResult>().FromSqlRaw("EXEC Billing_Create_Order_Item @OrderID, @ItemName, @Rate, @Unit, @Amount, @ChargeType",
                    pOrderID, pItemName, pRate, pUnit, pAmount, pChargeType).FirstOrDefaultAsync();

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
                        ProcessOrderTransactions(orderId, IP.CustomerId, IP.CurrentUserId, IP.ContractType);
                        ProcessPendingOrder(IP.OrderId, IP.CustomerId);
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
        private async Task<dynamic> GetOrder(int OrderId, int CompanyId)
        {
            try
            {
                var pOrderId = new SqlParameter("@OrderId", OrderId);
                var pCompanyId = new SqlParameter("@CompanyId", CompanyId);

                var result = _context.Set<OrderListReturn>().FromSqlRaw("EXEC Billing_Get_Order @OrderId, @CompanyId", pOrderId, pCompanyId).ToList();
                if (result != null)
                {
                    if (OrderId > 0)
                    {
                        var newresult = result.FirstOrDefault();
                        newresult.Modules = await GetProducts(OrderId, CompanyId);
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

        public async void ProcessOrderTransactions(int OrderID, int CompanyId, int CurrentUserId, string ContractType)
        {
            try
            {
                var cpp = (from CPP in _context.Set<CompanyPaymentProfile>() where CPP.CompanyId == CompanyId select CPP).FirstOrDefault();

                if (cpp != null)
                {
                    OrderListReturn order = await GetOrder(OrderID, CompanyId);

                    var cp = (from C in _context.Set<Company>() where C.CompanyId == CompanyId select C).FirstOrDefault();
                    if (cp != null)
                    {
                        string profile = "SUBSCRIBED";
                        if (cpp.MinimumBalance < cpp.CreditBalance)
                        {
                            profile = "LOW_BALANCE";
                        }
                        cp.CompanyProfile = profile;

                        if (order.ContractType == "NEW" || order.ContractType == "RENEW")
                        {
                            cp.OnTrial = false;
                            cpp.ContractStartDate = order.ContractStartDate;
                            cpp.ContractAnniversary = order.ContractStartDate.AddMonths(order.ContractDuration).AddDays(-1);

                            await _context.SaveChangesAsync();
                        }

                        string TimeZoneId = _DBC.GetTimeZoneByCompany(CompanyId);
                        DateTimeOffset dtnow = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);

                        decimal TotalValue = (decimal)(order.ContractValue + order.VatTotal);
                        decimal VATRate = (decimal)cpp.Vatrate;

                        int TotalAdmin = Convert.ToInt16(_DBC.GetPackageItem("ADMIN_USER_LIMIT", CompanyId));
                        int TotalStaff = Convert.ToInt16(_DBC.GetPackageItem("USER_LIMIT", CompanyId));

                        //Get storage details
                        int StorageLimit = Convert.ToInt16(_DBC.GetPackageItem("MEDIA_STORAGE", CompanyId));
                        var Storage = (from A in _context.Set<Assets>() where A.CompanyId == CompanyId select A).ToList();
                        double AssetSize = Storage.Select(s => s.AssetSize).Sum();

                        DateTime EndTime = order.ContractStartDate.AddMonths(order.ContractDuration).AddDays(-1);

                        int tran_header_id = await _usage.AddTransactionHeader(CompanyId, Convert.ToDecimal(order.ContractValue), VATRate, Convert.ToDecimal(order.VatTotal),
                            Convert.ToDecimal(TotalValue), 0, cpp.CreditLimit, order.KeyholderCount, TotalAdmin, order.StaffCount, TotalStaff, StorageLimit, AssetSize,
                            order.ContractStartDate, EndTime);


                        foreach (CompanyPackageFeatures item in order.Modules)
                        {

                            await UpdateTransactionDetails(tran_header_id, CompanyId, item.TransactionTypeID, Convert.ToDecimal(item.Rate), Convert.ToDecimal(item.Amount),
                                item.Units, Convert.ToDecimal(item.Amount), Convert.ToDecimal(item.Amount), 0, Convert.ToDecimal(item.Amount), item.ModuleID, order.OrderDate,
                                CurrentUserId, order.InvoiceNumber);

                            //Top up item
                            if (item.SecurityKey == "TOPUP")
                            {
                                decimal newBalance = cpp.CreditBalance + Convert.ToDecimal(item.Amount);
                                cpp.CreditBalance = newBalance;
                                cpp.LastCreditDate = dtnow;
                                cpp.UpdatedOn = dtnow;
                                cpp.UpdatedBy = CurrentUserId;
                                _context.SaveChangesAsync();
                                _DBC.GetSetCompanyComms(CompanyId);
                            }

                            //Handle the Storage Transaction
                            if (item.SecurityKey == "MEDIA_STORAGE")
                            {
                                if (ContractType == "NEW" || ContractType == "RENEW")
                                {
                                    cpp.StorageLimit = item.Units;
                                }
                                else
                                {
                                    cpp.StorageLimit += item.Units;
                                }
                                cpp.UpdatedOn = dtnow;
                                cpp.UpdatedBy = CurrentUserId;

                                var storage = (from CP in _context.Set<CompanyPackageItem>() where CP.ItemCode == "MEDIA_STORAGE" && CP.CompanyId == CompanyId select CP).FirstOrDefault();
                                if (storage != null)
                                {
                                    storage.ItemValue = cpp.StorageLimit.ToString();
                                    storage.UpdatedBy = CurrentUserId;
                                    storage.UpdatedOn = dtnow;
                                }

                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UpdateTransactionDetails(int TransactionHeaderId, int CompanyId, int TransactionTypeId, decimal TransactionRate, decimal MinimumPrice,
           int Quantity, decimal Cost, decimal LineValue, decimal LineVAT, decimal Total, int MessageId, DateTimeOffset TransactionDate, int currntUserId = 0,
           string TransactionReference = "", string TimeZoneId = "GMT Standard Time", int TransactionStatus = 1, int TransactionDetailsId = 0, string TrType = "DR")
        {

            int TDId = 0;
            try
            {
                if (Total > 0)
                {
                    if (TransactionDetailsId == 0)
                    {

                        TransactionDetail NewTransactionDetails = new TransactionDetail()
                        {
                            TransactionHeaderId = TransactionHeaderId,
                            CompanyId = CompanyId,
                            TransactionReference = TransactionReference,
                            TransactionTypeId = TransactionTypeId,
                            TransactionRate = TransactionRate,
                            MinimumPrice = MinimumPrice,
                            TransactionDate = TransactionDate,
                            Quantity = Quantity,
                            Cost = Cost,
                            LineValue = LineValue,
                            LineVat = LineVAT,
                            Total = Total,
                            TransactionStatus = TransactionStatus,
                            MessageId = MessageId,
                            CreatedBy = currntUserId,
                            CreatedOn = DateTime.Now,
                            UpdatedBy = currntUserId,
                            UpdateOn = DateTime.Now,
                            Drcr = TrType,
                            IsPaid = false
                        };
                        _context.Set<TransactionDetail>().Add(NewTransactionDetails);
                        await _context.SaveChangesAsync();
                        TDId = NewTransactionDetails.TransactionDetailsId;

                    }
                    else
                    {
                        var newTransactionDetails = (from TD in _context.Set<TransactionDetail>() where TD.TransactionDetailsId == TransactionDetailsId select TD).FirstOrDefault();
                        if (newTransactionDetails != null)
                        {
                            newTransactionDetails.TransactionHeaderId = TransactionHeaderId;
                            newTransactionDetails.TransactionReference = TransactionReference;
                            newTransactionDetails.TransactionTypeId = TransactionTypeId;
                            newTransactionDetails.TransactionRate = TransactionRate;
                            newTransactionDetails.MinimumPrice = MinimumPrice;
                            newTransactionDetails.Quantity = Quantity;
                            newTransactionDetails.LineValue = LineValue;
                            newTransactionDetails.Cost = Cost;
                            newTransactionDetails.LineVat = LineVAT;
                            newTransactionDetails.Total = Total;
                            newTransactionDetails.TransactionDate = TransactionDate;
                            newTransactionDetails.TransactionStatus = TransactionStatus;
                            newTransactionDetails.UpdatedBy = currntUserId;
                            newTransactionDetails.UpdateOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                            newTransactionDetails.Drcr = TrType;
                            _context.SaveChanges();
                            TDId = newTransactionDetails.TransactionDetailsId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return TDId;
        }

        public void ProcessPendingOrder(int OrderID, int CompanyId)
        {
            try
            {
                var pOrderId = new SqlParameter("@OrderId", OrderID);
                var pCompanyId = new SqlParameter("@CompanyId", CompanyId);

                var result = _context.Set<CompanyPackageFeatures>().FromSqlRaw("EXEC Process_Pending_Order @OrderId, @CompanyId", pOrderId, pCompanyId).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> CreateInvoiceSchedule(OrderModel orderModel)
        {
            try
            {
                int OrderID = await CreateInvoiceScheduleOrderHeader(orderModel.CompanyId, orderModel.CustomerId.ToString(), orderModel.OrderId, (double)orderModel.ContractValue, orderModel.ContractDuration, orderModel.OrderDate,
                orderModel.PaymentMethod, orderModel.KeyholderCount, orderModel.StaffCount, orderModel.TigerOrderNo, orderModel.InvoiceNumber, orderModel.OrderStatus, (double)orderModel.NetTotal, (double)orderModel.VatTotal, orderModel.ContractStartDate);

                if (OrderID > 0)
                {
                    foreach (Product item in orderModel.Products)
                    {
                        int OrderItemId = await CreateInvoiceScheduleOrderItem(OrderID, item.ItemName, (double)item.Rate, item.Unit, (double)item.Amount, item.ChargeType);
                        CreateSchedule(OrderItemId, (double)item.Amount, orderModel.ContractDuration, orderModel.ContractStartDate, item.ChargeType);
                    }
                }
                return OrderID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CreateSchedule(int orderItemId, double totalAmount, int period, DateTime contractStartDate, string chargeType)
        {
            try
            {
                var pOrderItemId = new SqlParameter("@OrderItemId", orderItemId);
                var pTotalAmount = new SqlParameter("@TotalAmount", totalAmount);
                var pPeriod = new SqlParameter("@NumberOfPayments", period);
                var pContractStartDate = new SqlParameter("@ContractStartDate", contractStartDate);
                var pChargeType = new SqlParameter("@ChargeType", chargeType);

                _context.Database.ExecuteSqlRaw("EXEC Pro_Billing_Create_Invoice_Schedule @OrderItemId, @TotalAmount, @NumberOfPayments, @ContractStartDate, @ChargeType",
                    pOrderItemId, pTotalAmount, pPeriod, pContractStartDate, pChargeType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<GetInvoiceByIdResponse> GetInvoicesById(int companyId, int transactionHeaderId, bool showPayments)
        {
            GetInvoiceByIdResponse? billingDetails = new GetInvoiceByIdResponse();
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var pTransactionHeaderID = new SqlParameter("@TranHeaderID", transactionHeaderId);
                billingDetails = await _context.Set<GetInvoiceByIdResponse>().FromSqlRaw(
                    "EXEC Pro_Billing_GetInvoicesByRef @CompanyID,@TranHeaderID",
                    pCompanyID,
                    pTransactionHeaderID).FirstOrDefaultAsync();

                if (billingDetails != null)
                {
                    var pCompanyID2 = new SqlParameter("@CompanyID", companyId);
                    var pTransactionHeaderID2 = new SqlParameter("@TranHeaderID", transactionHeaderId);
                    var pShowPayments = new SqlParameter("@ShowPayments", showPayments);
                    billingDetails.BillingDetailsInfoList = await _context.Set<BillingDetailsInfo>().FromSqlRaw(
                        "EXEC Pro_Billing_GetInvoicesByRef_BillingDetailsInfo @CompanyID,@TranHeaderID,@ShowPayments",
                        pCompanyID2,
                        pTransactionHeaderID2,
                        pShowPayments).ToListAsync();

                }
                else
                {
                    billingDetails.ErrorId = 110;
                    billingDetails.Message = "No record found.";
                }
                return billingDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<TransactionItemDetails>> GetTransactionItem(int companyId, int messageId, string method, int recordStart = 0, int recordLength = 100, string searchString = "",
            string orderBy = "Name", string orderDir = "asc", string companyKey = "")
        {
            try
            {

                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var pMessageID = new SqlParameter("@MessageID", messageId);
                var pMethod = new SqlParameter("@Method", method);
                var pRecordStart = new SqlParameter("@RecordStart", recordStart);
                var pRecordLength = new SqlParameter("@RecordLength", recordLength);
                var pSearchString = new SqlParameter("@SearchString", searchString);
                var pOrderBy = new SqlParameter("@OrderBy", orderBy);
                var pOrderDir = new SqlParameter("@OrderDir", orderDir);
                var pCompanyKey = new SqlParameter("@UniqueKey", companyKey);

                var propertyInfo = typeof(TransactionItemDetails).GetProperty(orderBy);
                var result = new List<TransactionItemDetails>();

                if (orderDir == "desc")
                {
                    result = await _context.Set<TransactionItemDetails>().FromSqlRaw(
                    "EXEC Pro_Billing_Transaction_Details @CompanyID, @MessageID, @Method, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir,@UniqueKey",
                    pCompanyID, pMessageID, pMethod, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pCompanyKey)
                    .OrderByDescending(o => propertyInfo.GetValue(o, null)).ToListAsync();
                }
                else
                {
                    result = await _context.Set<TransactionItemDetails>().FromSqlRaw(
                    "EXEC Pro_Billing_Transaction_Details @CompanyID, @MessageID, @Method, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir,@UniqueKey",
                    pCompanyID, pMessageID, pMethod, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pCompanyKey)
                    .OrderBy(o => propertyInfo.GetValue(o, null)).ToListAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<dynamic> GetUnbilledSummary(int startYear)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyId", companyId);
                var pStartYear = new SqlParameter("@StartYear", startYear);
                var result = await _context.Set<UnbilledSummary>().FromSqlRaw("EXEC Pro_Get_Unbilled_Transaction_Monthly_Summary @CompanyId, @StartYear",
                    pCompanyId, pStartYear).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<UsageGraph>> GetUsageGraph(int companyId, string reportType, int lastMonth)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var pReportType = new SqlParameter("@ReportType", reportType);
                var pLastMonth = new SqlParameter("@LastMonth", lastMonth);

                var result = await _context.Set<UsageGraph>().FromSqlRaw("EXEC Pro_Company_Comms_Usage @CompanyID, @ReportType, @LastMonth", pCompanyID, pReportType, pLastMonth).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<dynamic> GetUnbilledSummaryByMonth(int startYear, int monthNumber)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyId", companyId);
                var pStartYear = new SqlParameter("@StartYear", startYear);
                var pMonthNumber = new SqlParameter("@MonthNumber", monthNumber);

                var result = await _context.Set<UnbilledSummary>().FromSqlRaw("EXEC Pro_Get_Unbilled_Transaction_Summary_For_Month @CompanyId, @StartYear, @MonthNumber",
                    pCompanyId, pStartYear, pMonthNumber).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<dynamic> GetUnbilledSummaryByMessage(int messageId)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyId", companyId);
                var pMessageId = new SqlParameter("@MessageId", messageId);

                var result = await _context.Set<UnbilledSummary>().FromSqlRaw("EXEC Pro_Get_Unbilled_Transaction_Details_By_Message @CompanyId, @MessageId",
                    pCompanyId, pMessageId).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> UpdateCompanyTranscationType(int companyId, int currntUserId, string timeZoneId, int transactionTypeId, decimal transactionRate,
           int compnayTranscationTypeId = 0, string paymentPeriod = "MONTHLY", DateTimeOffset? nextRunDate = null, string paymentMethod = "INVOICE")
        {
            int CTTId = 0;
            if (compnayTranscationTypeId == 0)
            {
                CompanyTranscationType transaction = new CompanyTranscationType();
                if (transactionTypeId > 0)
                    transaction.TransactionTypeID = transactionTypeId;
                transaction.TransactionRate = transactionRate;
                transaction.CompanyId = companyId;
                transaction.PaymentPeriod = paymentPeriod;
                if (nextRunDate.HasValue)
                {
                    transaction.NextRunDate = (DateTimeOffset)nextRunDate;
                }
                else
                {
                    transaction.NextRunDate = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                }
                transaction.CreatedBy = currntUserId;
                transaction.CreatedOn = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                transaction.UpdatedBy = currntUserId;
                transaction.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);

                if (!string.IsNullOrEmpty(paymentMethod) && paymentMethod != "UNKNOWN")
                    transaction.PaymentMethod = paymentMethod;

                _context.Set<CompanyTranscationType>().Add(transaction);
                await _context.SaveChangesAsync();
                CTTId = transaction.CompanyTranscationTypeId;
            }
            else
            {
                var newCompanyTranscationType = await (from CTT in _context.Set<CompanyTranscationType>()
                                                 where CTT.CompanyTranscationTypeId == compnayTranscationTypeId && CTT.CompanyId == companyId
                                                 select CTT).FirstOrDefaultAsync();
                if (newCompanyTranscationType != null)
                {
                    if (transactionTypeId > 0)
                        newCompanyTranscationType.TransactionTypeID = transactionTypeId;

                    newCompanyTranscationType.TransactionRate = transactionRate;
                    newCompanyTranscationType.PaymentPeriod = paymentPeriod;
                    newCompanyTranscationType.PaymentMethod = paymentMethod;

                    if (nextRunDate.HasValue)
                    {
                        newCompanyTranscationType.NextRunDate = (DateTimeOffset)nextRunDate;
                    }
                    newCompanyTranscationType.UpdatedBy = currntUserId;
                    newCompanyTranscationType.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                    await _context.SaveChangesAsync();
                    CTTId = newCompanyTranscationType.CompanyTranscationTypeId;
                }
            }
            return CTTId;
        }
    }
}
