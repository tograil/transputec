
using CrisesControl.Core.Companies;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Core.Payments;
using CrisesControl.Core.Sop.Respositories;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services {
    public class UsageHelper {
        private decimal VATRate = 0M;
        private string CommsDebug = "true";
        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDBCommonRepository DBC;
        private readonly ISenderEmailService _SDE;
        private BillingHelper _billing ;

        public UsageHelper(CrisesControlContext context, IDBCommonRepository _DBC, ISenderEmailService SDE)
        {
            VATRate = 0M;
            _context = context;
            _httpContextAccessor = new HttpContextAccessor();
            DBC =_DBC;
            _SDE = SDE;
            decimal.TryParse(DBC.LookupWithKey("COMP_VAT").ToString(), out VATRate);
            CommsDebug = DBC.LookupWithKey("COMMS_DEBUG_MODE").ToString();
            _billing = new BillingHelper(_context, DBC,_SDE);


        }
        public async Task BalanceCheckNDebit(int CompanyID, decimal BalanceAfterCompute) {
            //Recharege the credit balance if balance falls down below the threshold.
            var comp_profile = await _context.Set<CompanyPaymentProfile>()
                                .Where(CPP => CPP.CompanyId == CompanyID).FirstOrDefaultAsync();

            if (comp_profile != null) {
                if (!string.IsNullOrEmpty(comp_profile.AgreementNo) && comp_profile.AgreementNo.Length > 4) {
                    decimal recharge_th = 10M;
                    decimal recharge_value = 0M;
                    decimal.TryParse(await DBC.GetCompanyParameter("RECHARGE_BALANCE_TRIGGER", CompanyID), out recharge_th);
                    decimal.TryParse(await DBC.GetCompanyParameter("CREDIT_BALANCE_RECHARGE", CompanyID), out recharge_value);

                    if (BalanceAfterCompute <= recharge_th) {

                        int ttype = _billing.GetTransactionTypeID("TOPUP").Result;

                        decimal TotalValueToRecharge = BalanceAfterCompute;
                        int NoOfTopUps = 0;

                        while (TotalValueToRecharge < recharge_th) {
                            TotalValueToRecharge += recharge_value;
                            NoOfTopUps++;
                        }

                        TotalValueToRecharge = NoOfTopUps * recharge_value;


                        decimal AmountLeftForDebit = TotalValueToRecharge;
                        decimal TotalAmountDebited = 0;
                        bool continuepayment = true;

                        int Attempt = 0;
                        int MaxAttempt = Convert.ToInt16(await DBC.LookupWithKey("WP_FAILED_PAYMENT_ATTEMPT"));
                        string CardFailedErrorResponse =await  DBC.LookupWithKey("WP_CARD_FAILED_RESPONSE");
                        string CardSuccessResponse =await DBC.LookupWithKey("WP_CARD_SUCCESS_RESPONSE");
                        string ResponseMessages = string.Empty;

                        while (AmountLeftForDebit > 0 && continuepayment == true && Attempt <= MaxAttempt) {

                            //Check if the transaction amount is greater then the agreement limit
                            //Then break the transaction into multiple
                            decimal TransactionAmount = AmountLeftForDebit;
                            if (TransactionAmount > comp_profile.MaxTransactionLimit) {
                                TransactionAmount = comp_profile.MaxTransactionLimit;
                            }

                            //initiate the worldpay transaction and record the status
                            PaymentResponse response = CompanyPaymentTranaction(CompanyID, comp_profile.AgreementNo, TransactionAmount).Result;

                            if (response != null)
                            {
                                if (response.ResponseCode.ToUpper() == CardSuccessResponse)
                                {
                                    
                                    comp_profile.CreditBalance += response.Amount;
                                    TotalAmountDebited += TransactionAmount;
                                    AmountLeftForDebit -= TransactionAmount;
                                    await _context.SaveChangesAsync();

                                    _ = _billing.UpdateTransactionDetails(0, CompanyID, ttype, TransactionAmount, TransactionAmount, 1,
                                         TransactionAmount, TransactionAmount, 0, TransactionAmount, 0, DateTime.Now, 0, response.TransactionID, "GMT Standard Time", 1, 0, "CR");

                                    //Send the debit success alert
                                    if (AmountLeftForDebit <= 0)
                                    {
                                        if (CommsDebug == "true")
                                        {
                                           await  DBC.CreateLog("INFO", "Payment Transaction Success Alert, amount " + TotalAmountDebited, null, "UsageHelper", "BalanceCheckNDebIt", CompanyID);
                                        }
                                        else
                                        {
                                            string TimeZoneId =await  DBC.GetTimeZoneByCompany(CompanyID);
                                            await _SDE.SendPaymentTransactionAlert(CompanyID, TotalAmountDebited, TimeZoneId);
                                        }

                                        continuepayment = false;
                                    }
                                    //} else if(response.ResponseCode.ToUpper() == "ERROR") {
                                } else {
                                    Attempt++;
                                    ResponseMessages += (string.IsNullOrEmpty(ResponseMessages) ? "," : "") + response.ResponseMessage;

                                    await DBC.CreateLog("INFO", response.ResponseMessage, null, "UsageHelper", "BalanceCheckNDebit", CompanyID);

                                    if (Attempt > MaxAttempt) {
                                        if (CommsDebug == "true") {
                                            DBC.CreateLog("INFO", "Payment Transaction Failure Alert " + TransactionAmount + " Error:" + ResponseMessages, null, "UsageHelper", "BalanceCheckNDebIt", CompanyID);
                                        }
                                        else
                                        {
                                           
                                           await  _SDE.SendFailedPaymentAlert(CompanyID, TransactionAmount, ResponseMessages);
                                        }

                                        _ = _billing.UpdateTransactionDetails(0, CompanyID, ttype, 0, TransactionAmount, 1,
                                        0, 0, 0, 0, 0, DateTime.Now, 0, DBC.Left(ResponseMessages, 150), "GMT Standard Time", 1, 0, "ER");

                                        if (response.ResponseCode.ToUpper() == CardFailedErrorResponse) {
                                            comp_profile.CardExpiryDate = DateTime.Now.AddMonths(-1);
                                            comp_profile.CardFailed = true;
                                            await _context.SaveChangesAsync();
                                            Attempt = MaxAttempt + 1;
                                        }

                                        continuepayment = false;
                                    }
                                }
                            }
                        }

                        await DBC.GetSetCompanyComms(CompanyID);
                    }
                }
            }
        }
        public async Task<PaymentResponse> CompanyPaymentTranaction(int CompanyID, string AgreementNo = "", decimal RechargeAmount = 0M, string TransactinRef = "") {
            try {
                decimal recharge_value = 10;
                string wp_amdin_url =await  DBC.LookupWithKey("WP_IADMIN_URL");
                string wp_inst_id =await DBC.LookupWithKey("WP_IADMIN_INST_ID");
                string wp_auth_pwd =await DBC.LookupWithKey("WP_IADMIN_AUTH_PWD");

                string agreement_no = "";

                if (string.IsNullOrEmpty(AgreementNo) && CompanyID > 0) {
                    var comp_pp = await _context.Set<CompanyPaymentProfile>().Where(CPP => CPP.CompanyId == CompanyID).FirstOrDefaultAsync();
                    agreement_no = comp_pp.AgreementNo;
                } else {
                    agreement_no = AgreementNo;
                }

                if (RechargeAmount == 0 && CompanyID > 0)
                {
                    bool check = decimal.TryParse(await DBC.GetCompanyParameter("CREDIT_BALANCE_RECHARGE", CompanyID), out recharge_value);
                }
                else
                {
                    recharge_value = RechargeAmount;
                }

                if (CommsDebug == "true") {
                    return PrepareResponse("Y,1234,A,Payment success,Payment successful", recharge_value);
                }

                if (agreement_no != "" && agreement_no.Length >= 4) {
                    using (WebClient client = new WebClient()) {
                        try {
                            ServicePointManager.SecurityProtocol =
                                            SecurityProtocolType.Tls
                                            | SecurityProtocolType.Tls11
                                            | SecurityProtocolType.Tls12
                                            | SecurityProtocolType.Ssl3;
                            byte[] response =
                           client.UploadValues(wp_amdin_url, new NameValueCollection()
                                      {
                                        { "instId", wp_inst_id },
                                        { "authPW", wp_auth_pwd },
                                        { "futurePayId", agreement_no },
                                        { "amount", recharge_value.ToString() },
                                        { "desc", TransactinRef},
                                        { "op-paymentLFP", ""},
                                      });
                            string result = Encoding.UTF8.GetString(response);
                            return PrepareResponse(result, recharge_value);
                        } catch (WebException ex) {
                            throw ex;
                        }
                    }
                }
                return PrepareResponse("");
            } catch (Exception ex) {
                throw ex;
            }
        }

        public PaymentResponse PrepareResponse(string result, decimal Amount = 0M) {
            PaymentResponse PR = new PaymentResponse();
            try {
                if (!string.IsNullOrEmpty(result)) {
                    string[] resultset = result.Split(',');
                    if (resultset.Length > 2) {
                        PR.ResponseCode = resultset[0];
                        PR.TransactionID = resultset[1];
                        PR.AuthCode = resultset[2];
                        PR.RawMessage = resultset[3];
                        PR.ResponseMessage = resultset[4];
                    } else {
                        PR.ResponseCode = resultset[0];
                        PR.ResponseMessage = resultset[1];
                    }
                    PR.Amount = Amount;
                }
                return PR;
            } catch (Exception ex) {

                PR.ResponseCode = "ERROR";
                return PR;
            }
        }
        public async Task GenerateProrataInvoice(int CompanyID) {
            try {
                var profile = await _context.Set<CompanyPaymentProfile>().Where(CPP => CPP.CompanyId == CompanyID).FirstOrDefaultAsync();
                if (profile != null) {
                    int TransactionTypeID = await _billing.GetTransactionTypeID("LICENSEFEE");
                    var comp_trans = await _context.Set<CompanyTransactionType>().Where(w => w.CompanyId == CompanyID && w.TransactionTypeId == TransactionTypeID).FirstOrDefaultAsync();
                    if (comp_trans != null) {
                        decimal contract_value = comp_trans.TransactionRate;
                        decimal prorata_value = GetProrataPayment(contract_value, profile.ContractStartDate, profile.PaymentPeriod);

                        int trans_id = await _billing.AddMonthTransaction(0, CompanyID, 0, "PRORATALICENSEFEE", prorata_value, TransactionTypeID, profile.ContractStartDate);
                        await _billing.UpdateThisMonthOnly(trans_id, true, true);
                        DateTime NextAnniversary = LastDayofMonth(profile.ContractStartDate);
                        profile.ContractAnniversary = NextAnniversary;
                        comp_trans.NextRunDate = NextAnniversary;
                        await _context.SaveChangesAsync();


                        var reprofile = await _context.Set<CompanyPaymentProfile>().Where(CPP => CPP.CompanyId == CompanyID).FirstOrDefaultAsync();

                        //Charge or generate the invoice
                       await AddMonthlyItemsToTransactions(CompanyID);
                        if (comp_trans.PaymentMethod == "CREDIT_CARD")
                        {
                           await DebitAccount(CompanyID);
                        }
                        else
                        { //send email to admin for collecting payment offline by invoice
                            
                            await _SDE.InvoicePaymentAlert(CompanyID, prorata_value);
                        }
                    }

                }
            } catch (Exception ex) {
                throw ex;
            }
        }
        public decimal PerDayCharge(decimal BaseValue) {
            return (BaseValue * 12) / 365;
        }

        public DateTime LastDayofMonth(DateTimeOffset DateVal) {
            DateTime FirstDay = new DateTime(DateVal.Year, DateVal.Month, 1);
            DateTime LastDay = FirstDay.AddMonths(1).AddDays(-1);
            return LastDay;
        }

        public decimal GetProrataPayment(decimal BasePrice, DateTimeOffset EntityDate, string Period = "MONTHLY") {
            decimal ProrataPrice = BasePrice;
            try {
                decimal per_day_rate = 0;
                if (Period == "MONTHLY") {
                    per_day_rate = (BasePrice * 12) / 365;
                } else if (Period == "YEARLY") {
                    per_day_rate = BasePrice / 365;
                }
                DateTime firstDayOfMonth = new DateTime(EntityDate.Year, EntityDate.Month, 1);
                DateTimeOffset lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                double days_left_in_month = lastDayOfMonth.Subtract(EntityDate).TotalDays + 1;
                ProrataPrice = per_day_rate * (decimal)days_left_in_month;
                ProrataPrice = decimal.Round(ProrataPrice, 2);
            } catch (Exception) {
                throw;
            }
            return ProrataPrice;
        }
        public async Task DebitAccount(int CompanyID) {
            try {
                var comp_profile = await _context.Set<CompanyPaymentProfile>()
                                    .Where(CPP => CPP.CompanyId == CompanyID).FirstOrDefaultAsync();
                if (comp_profile != null) {
                    if (!string.IsNullOrEmpty(comp_profile.AgreementNo) && comp_profile.AgreementNo.Length > 4) {
                        decimal NetTotal = 0M;

                        var monthly_items = (from MT in _context.Set<MonthlyTransactionItem>()
                                             join TT in _context.Set<TransactionType>() on MT.TransactionTypeId equals TT.TransactionTypeId
                                             where MT.CompanyId == CompanyID
                                             select new { MT, TT }).ToList();
                        string email_items = "<table width=\"100%\"><tbody><tr><th style=\"margin: 0;font-size:14px;\"><strong>Description</strong></th>";
                        email_items += "<th style=\"margin:0;font-size:16px;line-height:21px;text-align:right;\"><strong>Amount</strong></th></tr>";

                        foreach (var item in monthly_items) {
                            NetTotal += item.MT.ItemValue;
                            email_items += "<tr><td class=\"auto-style1\">" + item.TT.TransactionDescription + "</td><td style=\"text-align:right\">&pound; " + item.MT.ItemValue + "</td></tr>";
                        }

                        email_items += "<tr><td class=\"auto-style1\" style=\"text-align:right;\">VAT: </td><td style=\"text-align:right\">{VAT_VALUE}</td></tr>";
                        email_items += "<tr><td class=\"auto-style1\" style=\"text-align:right;\">Total: </td><td style=\"text-align:right\">{TOTAL_PAYMENT_WITH_VAT}</td></tr>";
                        email_items += "</tbody></table>";
                        //Apply VAT amount
                        VATRate = (decimal)comp_profile.Vatrate;

                        decimal VatAmount = (NetTotal * VATRate) / 100;
                        decimal TotalMonthlyDebitAmount = NetTotal + VatAmount;
                        TotalMonthlyDebitAmount = DecimalToMoney(TotalMonthlyDebitAmount);

                        decimal AmountLeftForDebit = TotalMonthlyDebitAmount;
                        decimal TotalAmountDebited = 0;
                        bool continuepayment = true;

                        int ttype = await _billing.GetTransactionTypeID("MONTHLYPAYMENT");

                        int Attempt = 0;
                        int MaxAttempt = Convert.ToInt16(DBC.LookupWithKey("WP_FAILED_PAYMENT_ATTEMPT"));

                        string CardFailedErrorResponse =await  DBC.LookupWithKey("WP_CARD_FAILED_RESPONSE");
                        string CardSuccessResponse =await  DBC.LookupWithKey("WP_CARD_SUCCESS_RESPONSE");
                        string ResponseMessages = string.Empty;


                        while (AmountLeftForDebit > 0 && continuepayment == true && Attempt <= MaxAttempt) {

                            //Check if the transaction amount is greater then the agreement limit
                            //Then break the transaction into multiple
                            decimal TransactionAmount = AmountLeftForDebit;
                            if (TransactionAmount > comp_profile.MaxTransactionLimit) {
                                TransactionAmount = comp_profile.MaxTransactionLimit;
                            }

                            //initiate the worldpay transaction and record the status
                            PaymentResponse response = await CompanyPaymentTranaction(CompanyID, comp_profile.AgreementNo, TransactionAmount);

                            if (response != null) {
                                if (response.ResponseCode.ToUpper() == CardSuccessResponse) {
                                    TotalAmountDebited += TransactionAmount;
                                    AmountLeftForDebit -= TransactionAmount;
                                    await _billing.UpdateTransactionDetails(0, CompanyID, ttype, NetTotal, NetTotal, 1,
                                            NetTotal, NetTotal, VatAmount, TransactionAmount, 0, DateTime.Now, 0, response.TransactionID, "GMT Standard Time", 1, 0, "CR");

                                    //} else if(response.ResponseCode.ToUpper() == "ERROR") {
                                    //todo - dump the transaction details  error log with 0 value.
                                } else {
                                    //todo - remove the above else if and send the email on the last attempt, also capture the logs.
                                    //SendEmail SDE = new SendEmail();
                                    //SDE.SendFailedPaymentAlert(CompanyID, TransactionAmount, response.ResponseMessage);

                                    Attempt++;
                                    ResponseMessages += (string.IsNullOrEmpty(ResponseMessages) ? "," : "") + response.ResponseMessage;

                                    await DBC.CreateLog("INFO", response.ResponseMessage, null, "UsageHelper", "DebitAccount", CompanyID);

                                    await _billing.UpdateTransactionDetails(0, CompanyID, ttype, 0, TransactionAmount, 1,
                                            0, 0, 0, 0, 0, DateTime.Now, 0, DBC.Left(ResponseMessages, 150), "GMT Standard Time", 1, 0, "ER");

                                    if (Attempt > MaxAttempt) {
                                        if (response.ResponseCode.ToUpper() == CardFailedErrorResponse) {
                                            comp_profile.CardExpiryDate = DateTime.Now.AddMonths(-1);
                                            comp_profile.CardFailed = true;
                                            _context.Update(comp_profile);
                                            await _context.SaveChangesAsync();
                                        }

                                        continuepayment = false;
                                    }
                                }
                            }
                        } //End while loop


                        if (TotalMonthlyDebitAmount == TotalAmountDebited) {

                            if (CommsDebug == "true")
                            {
                                await DBC.CreateLog("INFO", "Monthly payment success alert:" + TotalAmountDebited, null, "UsageHelper", "BalanceCheckNDebit", CompanyID);
                            }
                            else
                            {
                               
                                await _SDE.SendMonthlyPaymentAlert(CompanyID, TotalMonthlyDebitAmount, TotalAmountDebited, VatAmount, email_items);
                            }


                        }
                        else if (TotalAmountDebited < TotalMonthlyDebitAmount && TotalAmountDebited > 0)
                        {
                            if (CommsDebug == "true")
                            {
                                await DBC.CreateLog("INFO", "Monthly partial payment success alert:" + TotalAmountDebited, null, "UsageHelper", "BalanceCheckNDebit", CompanyID);
                            }
                            else
                            {
                               
                                await _SDE.SendMonthlyPartialPaymentAlert(CompanyID, TotalMonthlyDebitAmount, TotalAmountDebited, VatAmount, email_items);
                            }

                        }
                        else if ((Attempt > MaxAttempt) || TotalAmountDebited == 0)
                        {
                            if (CommsDebug == "true")
                            {
                               await DBC.CreateLog("INFO", "Monthly payment failure alert:" + ResponseMessages, null, "UsageHelper", "BalanceCheckNDebit", CompanyID);
                            }
                            else
                            {
                                
                               await _SDE.SendFailedPaymentAlert(CompanyID, TotalMonthlyDebitAmount, ResponseMessages);
                            }

                            //check if no amount is collected.. and attemt is maxattempt,; dump the error and send email.
                        }
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }

        }

        public decimal DecimalToMoney(decimal Amount, int DecimalPlace = 2) {
            return decimal.Round(Amount, DecimalPlace, MidpointRounding.AwayFromZero);
        }
        public async Task<int> AddTransactionHeader(int CompanyId, decimal NetTotal, decimal VatRate, decimal VatAmount, decimal Total, decimal CreditBalance, decimal CreditLimit,
           int AdminLimit, int AdminUsers, int StaffLimit, int StaffUsers, int StorageLimit, double StorageSize, DateTimeOffset StatementStartDate, DateTimeOffset StatementEndDate) {
            try {
                TransactionHeader NewTransactionHeader = new TransactionHeader() {
                    CompanyId = CompanyId,
                    NetTotal = NetTotal,
                    VatRate = VatRate,
                    Vatvalue = VatAmount,
                    Total = Total,
                    CreditBalance = CreditBalance,
                    CreditLimit = CreditLimit,
                    AdminLimit = AdminLimit,
                    AdminUsers = AdminUsers,
                    StaffLimit = StaffLimit,
                    StaffUsers = StaffUsers,
                    StorageLimit = StorageLimit,
                    StorageSize = StorageSize,
                    StatementDate = DateTime.Now,
                    TransactionStartDate = StatementStartDate,
                    TransactionEndDate = StatementEndDate,
                    CreatedBy = 0,
                    CreatedOn = DateTime.Now,
                    UpdatedBy = 0,
                    UpdatedOn = DateTime.Now
                };
                await _context.AddAsync(NewTransactionHeader);
                await _context.SaveChangesAsync();
                return NewTransactionHeader.TransactionHeaderId;
                //}

            } catch (Exception ex) {
                throw ex;
            }
            return 0;
        }

        public async Task AddMonthlyItemsToTransactions(int CompanyID) {
            try {
                var billing_items = await _context.Set<MonthlyTransactionItem>().Where(MT => MT.CompanyId == CompanyID).ToListAsync();
                decimal VATRate = await _context.Set<CompanyPaymentProfile>().Where(CP => CP.CompanyId == CompanyID).Select(CP => (decimal)CP.Vatrate).FirstOrDefaultAsync();
                decimal LineVat = 0M;
                decimal TotalLineValue = 0M;
                foreach (var item in billing_items) {

                    LineVat = (item.ItemValue * VATRate) / 100;
                    LineVat = DecimalToMoney(LineVat);

                    TotalLineValue = item.ItemValue + LineVat;
                    TotalLineValue = DecimalToMoney(TotalLineValue);

                    string TransactionRef = string.Empty;
                    if (item.UserRole == "PRORATALICENSEFEE")
                        TransactionRef = "Prorata license fee";

                    int Transid = await _billing.UpdateTransactionDetails(0, CompanyID, item.TransactionTypeId, item.ItemValue, item.ItemValue, 1, item.ItemValue,
                        item.ItemValue, LineVat, TotalLineValue, 0, item.TransactionDate, 0, TransactionRef);

                    if (item.ThisMonthOnly == true)
                        _context.Remove(item);
                }
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task _calculate_contract_items(int CompanyID)
        {
           
            try
            {
                var items =await (from CT in _context.Set<CompanyTransactionType>()
                             join TT in _context.Set<TransactionType>() on CT.TransactionTypeId equals TT.TransactionTypeId
                             where CT.CompanyId == CompanyID && TT.TransactionCode != "LICENSEFEE"
                             select CT).ToListAsync();

                var profile = await _context.Set<CompanyPaymentProfile>().Where(CP => CP.CompanyId == CompanyID).FirstOrDefaultAsync();
                DateTime firstDayOfMonth = DateTime.Now;
                DateTime lastDayOfMonth = DateTime.Now;
                DateTimeOffset TransactionDate = DateTimeOffset.Now;
                foreach (var item in items) {

                    TransactionDate = item.CreatedOn;

                    decimal this_month_charge = 0M;
                    if (item.NextRunDate <= DateTime.Now) {

                        var monthly_item = await _context.Set<MonthlyTransactionItem>()
                                            .Where(MT => MT.TransactionTypeId == item.TransactionTypeId
                                            && MT.CompanyId == CompanyID && MT.ThisMonthOnly == false).FirstOrDefaultAsync();

                        if (monthly_item == null) {
                            if (item.CreatedOn.Date <= DateTime.Now.Date && item.CreatedOn.Date == item.NextRunDate.Date) {
                                this_month_charge = GetProrataPayment(item.TransactionRate, item.NextRunDate, item.PaymentPeriod);
                                int mt_trans_id = await _billing.AddMonthTransaction(0, CompanyID, 0, "", this_month_charge, item.TransactionTypeId, TransactionDate);
                                _billing.UpdateThisMonthOnly(mt_trans_id, true, true);

                                DateTime NextAnniversary = LastDayofMonth(profile.CreatedOn);
                                item.NextRunDate = NextAnniversary;
                                _context.Update(item);
                                await _context.SaveChangesAsync();

                            } else {
                                TransactionDate = LastDayofMonth(item.NextRunDate).AddDays(1);

                                if (item.PaymentPeriod == "MONTHLY")
                                {
                                    int mt_trans_id = await _billing.AddMonthTransaction(0, CompanyID, 0, "", item.TransactionRate, item.TransactionTypeId, TransactionDate);
                                    _billing.UpdateThisMonthOnly(mt_trans_id, true, false);
                                    DateTimeOffset next_run_date = _billing.GetNextRunDate(item.NextRunDate, item.PaymentPeriod.ToUpper(), 0);
                                    item.NextRunDate = next_run_date;
                                }
                                else if (item.PaymentPeriod == "YEARLY")
                                {
                                    int mt_trans_id =await _billing.AddMonthTransaction(0, CompanyID, 0, "", item.TransactionRate, item.TransactionTypeId, TransactionDate);
                                    _billing.UpdateThisMonthOnly(mt_trans_id, true, true);
                                    DateTimeOffset next_run_date = _billing.GetNextRunDate(profile.ContractAnniversary, item.PaymentPeriod.ToUpper(), 0);
                                    item.NextRunDate = next_run_date;
                                }

                                await _context.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            DateTimeOffset next_run_date = _billing.GetNextRunDate(item.NextRunDate, item.PaymentPeriod.ToUpper(), 0);
                            if (monthly_item.ThisMonthOnly)
                            {
                                if (profile.PaymentPeriod.ToUpper() == "MONTHLY")
                                {
                                    _billing.UpdateThisMonthOnly(monthly_item.TransactionId, true, false);
                                    monthly_item.ItemValue = item.TransactionRate;
                                } else if (item.PaymentPeriod.ToUpper() == "YEARLY") {
                                    _context.Remove(monthly_item);
                                    next_run_date = _billing.GetNextRunDate(profile.ContractAnniversary, item.PaymentPeriod.ToUpper(), 0);
                                }
                            }


                            monthly_item.TransactionDate = next_run_date;
                            item.NextRunDate = next_run_date;
                            _context.Update(item);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<bool> CheckContractFee(int CompanyID, DateTimeOffset ContractStartDate, DateTimeOffset ContractAnniversary, DateTimeOffset StatementEndTime, decimal ContractValue) {
            ContractValue = 0M;
            try {
                int TransactionTypeID = await _billing.GetTransactionTypeID("LICENSEFEE");
                var contract = await _context.Set<MonthlyTransactionItem>()
                                .Where(MT => MT.CompanyId == CompanyID && MT.TransactionTypeId == TransactionTypeID).FirstOrDefaultAsync();

                var comp_profile = await _context.Set<CompanyPaymentProfile>().Where(w => w.CompanyId == CompanyID).FirstOrDefaultAsync();
                var comp_trans = await _context.Set<CompanyTransactionType>().Where(w => w.CompanyId == CompanyID && w.TransactionTypeId == TransactionTypeID).FirstOrDefaultAsync();
                int PackagePlan = (int)await _context.Set<Company>().Where(w => w.CompanyId == CompanyID).Select(s => s.PackagePlanId).FirstOrDefaultAsync();

                decimal contract_value = comp_trans.TransactionRate;

                DateTimeOffset next_run_date = new DateTime(comp_profile.ContractAnniversary.Year, comp_profile.ContractAnniversary.Month, 1);
                next_run_date = _billing.GetNextRunDate(comp_profile.ContractAnniversary, comp_profile.PaymentPeriod.ToUpper());

                DateTimeOffset TransactionDate = comp_profile.CurrentStatementEndDate.AddDays(1);


                if (ContractAnniversary.Date <= DateTimeOffset.Now.Date) {

                    if (contract == null) {

                        if (comp_profile.PaymentPeriod.ToUpper() == "MONTHLY") {
                            int trans_id = await _billing.AddMonthTransaction(0, CompanyID, 0, "LICENSEFEE", contract_value, TransactionTypeID, TransactionDate);
                            await _billing.UpdateThisMonthOnly(trans_id, true, false);
                        } else if (comp_profile.PaymentPeriod.ToUpper() == "YEARLY") {
                            int trans_id = await _billing.AddMonthTransaction(0, CompanyID, 0, "LICENSEFEE", contract_value, TransactionTypeID, TransactionDate);
                            await _billing.UpdateThisMonthOnly(trans_id, true, true);
                        }
                        comp_profile.ContractAnniversary = next_run_date;
                        _context.Update(comp_profile);
                        await _context.SaveChangesAsync();

                        ContractValue = DecimalToMoney(contract_value);
                        return true;
                    } else if (contract != null) {
                        if (contract.ThisMonthOnly) {
                            if (comp_profile.PaymentPeriod.ToUpper() == "MONTHLY") {
                                if (contract.UserRole == "PRORATALICENSEFEE") {
                                    _context.Remove(contract);
                                } else {
                                    await _billing.UpdateThisMonthOnly(contract.TransactionId, true, false);
                                    contract.ItemValue = contract_value;
                                }
                            } else if (comp_profile.PaymentPeriod.ToUpper() == "YEARLY") {
                                _context.Remove(contract);
                            }
                        }
                        comp_profile.ContractAnniversary = next_run_date;
                        contract.TransactionDate = TransactionDate;
                        //_context.Update();
                        await _context.SaveChangesAsync();
                        ContractValue = contract_value;
                        return true;
                    }
                }
                return false;
            } catch (Exception ex) {
                throw ex;
            }
            return false;
        }
        public async Task<LicenseCheckResult> GetUserLicenseInfo(int companyId, List<UserRoles> userList) {
            LicenseCheckResult Rslt = new LicenseCheckResult();
            try {
                int UserLimit = 0;
                int AdminUserLimit = 0;
                double UserRate = 0;
                double AdminUserRate = 0;

                var comp_pp = await _context.Set<CompanyPaymentProfile>().Include(x => x.Company)
                                   .Where(CPP => CPP.CompanyId == companyId)
                                   .Select(CPP => new { CPP.PaymentPeriod, CPP.Company.CompanyProfile, CPP.Company.OnTrial }).FirstOrDefaultAsync();

                int.TryParse(await DBC.GetPackageItem("USER_LIMIT", companyId), out UserLimit);
                int.TryParse(await DBC.GetPackageItem("ADMIN_USER_LIMIT", companyId), out AdminUserLimit);
                double.TryParse(await DBC.GetPackageItem("USER_RATE", companyId), out UserRate);
                double.TryParse(await DBC.GetPackageItem("ADMIN_USER_RATE", companyId), out AdminUserRate);

                int _keyholder_count = 0;
                int _staff_count = 0;
                int new_keyholder_count = 0;
                int new_staff_count = 0;

                var roles =await DBC.CCRoles(true);

                List<int> UserIds = new List<int>();
                foreach (UserRoles ur in userList) {
                    UserIds.Add(ur.UserId);
                    if (string.IsNullOrEmpty(ur.UserRole))
                        ur.UserRole = "USER";

                    if (roles.Contains(ur.UserRole.ToUpper())) {
                        _keyholder_count++;
                    } else {
                        _staff_count++;
                    }
                }

                var users = await _context.Set<User>().Where(U => U.CompanyId == companyId && U.Status != 3).ToListAsync();

                int total_existing_kh = users.Where(TK => (roles.Contains(TK.UserRole.ToUpper()))).Count();
                int total_existing_staff = users.Where(TK => TK.UserRole.ToUpper() == "USER").Count();

                int keyholders = users
                                  .Where(KH => (roles.Contains(KH.UserRole.ToUpper())) &&
                                  !UserIds.Contains(KH.UserId)).Count();

                int staff = (from KH in users where KH.UserRole.ToUpper() == "USER" && !UserIds.Contains(KH.UserId) select KH).Count();

                new_staff_count = _staff_count + staff;
                new_keyholder_count = _keyholder_count + keyholders;

                int extra_keyholder = new_keyholder_count - Math.Max(total_existing_kh, AdminUserLimit);
                int extra_staff = new_staff_count - Math.Max(total_existing_staff, UserLimit);

                if ((extra_keyholder > 0) || extra_staff > 0) {
                    Rslt.ConfirmAction = true;
                } else {
                    Rslt.ConfirmAction = false;
                }

                Rslt.PerKeyholderCost = AdminUserRate;
                Rslt.PerStaffCost = UserRate;
                Rslt.StaffLimit = UserLimit;
                Rslt.KeyholderLimit = AdminUserLimit;
                Rslt.ExtraKeyholders = extra_keyholder;
                Rslt.ExtraStaff = extra_staff;
                Rslt.Duration = comp_pp.PaymentPeriod;
                Rslt.CompanyProfile = comp_pp.CompanyProfile;
                Rslt.OnTrial = comp_pp.OnTrial;

            } catch (Exception ex) {

                Rslt.ConfirmAction = false;
            }
            return Rslt;
        }


    }
}
