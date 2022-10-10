
using CrisesControl.Core.Companies;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public class UsageCalculation
    {
        private readonly CrisesControlContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDBCommonRepository DBC;
        private readonly ISenderEmailService SDE;
        private decimal VATRate = 0M;
        private readonly BillingHelper _billing;

        private readonly UsageHelper _usage;
        private string CommsDebug = "true";
        private int SMSLogRetryInterval = 125;
        private string SMSStatusToCharge = "";
        private string PhoneStatusToCharge = "";

        public UsageCalculation(IDBCommonRepository _DBC, ISenderEmailService _SDE)
        {
            _httpContextAccessor = new HttpContextAccessor();
            DBC = _DBC;
            SDE = _SDE;
            _usage= new UsageHelper(db,DBC,SDE);
        }
        public async Task update_company_balance(int CompanyId, decimal TransactionValue)
        {
            try
            {
                decimal CreditLimit = 0;
                decimal CreditBalance = 0;
                decimal MinimumBalance = 0;
                decimal BalanceAfterCompute = 0;

                var comp_profile =  db.Set<CompanyPaymentProfile>()
                                   .Where(CPP=> CPP.CompanyId == CompanyId)
                                   .FirstOrDefault();
                if (comp_profile != null)
                {

                    //Update the company balance
                    CreditLimit = comp_profile.CreditLimit;
                    CreditBalance = comp_profile.CreditBalance;
                    MinimumBalance = comp_profile.MinimumBalance;

                    BalanceAfterCompute = CreditBalance - TransactionValue;

                    comp_profile.CreditBalance = BalanceAfterCompute;
                    db.Update(comp_profile);
                   await db.SaveChangesAsync();

                   _ = DBC.GetSetCompanyComms(CompanyId);

                    //Check the remaining balance and recharge
                    if (comp_profile.ContractStartDate <= DateTime.UtcNow)
                    {
                        await _usage.BalanceCheckNDebit(comp_profile.CompanyId, BalanceAfterCompute);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task Generate_Statement()
        {
            try
            {
                DateTime runDate = DateTime.Now.Date;

                var Companies = await (from C in db.Set<Company>()
                                 join CPP in db.Set<CompanyPaymentProfile>() on C.CompanyId equals CPP.CompanyId
                                 where C.Status == 1
                                 select new { C, CPP }).ToListAsync();

                var payment_profile =  await db.Set<PaymentProfile>().FirstOrDefaultAsync();

                foreach (var comp in Companies)
                {
                    try
                    {

                        //Call the function 
                        //stodo check the comparison
                        //DBC.CreateLog("INFO", "Total Company to bill"+Companies.Count);
                        if (comp.CPP.ContractStartDate.Date <= DateTime.Now.Date && comp.CPP.ContractAnniversary.Date == comp.CPP.ContractStartDate.Date)
                            //Call the function.
                            _ = _usage.GenerateProrataInvoice(comp.C.CompanyId);

                        DateTimeOffset stmt_run_date = comp.CPP.StatementRunDate;
                        DateTimeOffset stmt_start_date = comp.CPP.LastStatementEndDate;
                        DateTimeOffset stmt_end_date = comp.CPP.CurrentStatementEndDate;

                        //DBC.CreateLog("INFO", "Checking the dates agains" + stmt_run_date+"|"+ stmt_start_date+ "|" + stmt_end_date);
                        if (runDate.Date >= stmt_start_date.Date && runDate.Date <= stmt_end_date.Date)
                        {
                            continue;
                        }
                        //DBC.CreateLog("INFO", "Passed the check" + stmt_run_date + "|" + stmt_start_date + "|" + stmt_end_date);

                        //Run the statement for the last calendar month from the statement date.
                        _ = _usage._calculate_contract_items(comp.C.CompanyId);

                        _ = _generate_company_statement(comp.C.CompanyId, stmt_start_date, stmt_end_date.AddHours(23).AddMinutes(59).AddSeconds(59));

                        comp.CPP.StatementRunDate = stmt_run_date.AddMonths(1);
                        DateTimeOffset new_start_date = stmt_end_date.AddDays(1);
                        comp.CPP.LastStatementEndDate = new_start_date;
                        comp.CPP.CurrentStatementEndDate = new_start_date.AddMonths(1).AddDays(-1);

                        DBC.CreateLog("INFO", "Statement generated successfully" + stmt_run_date + "|" + stmt_start_date + "|" + stmt_end_date);

                        await db.SaveChangesAsync();

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task _generate_company_statement(int CompanyId, DateTimeOffset StartTime, DateTimeOffset EndTime)
        {
            try
            {
                decimal TotalTransactionValue = 0M;
                decimal TotalVatValue = 0M;
                decimal TotalTotal = 0M;
                decimal CreditLimit = 0M;
                decimal CreditBalance = 0M;
                decimal MinimumBalance = 0M;
                decimal TotalPayments = 0M;

                //Get company payment profile containing all the pricing and rates
                var comp_profile = await db.Set<CompanyPaymentProfile>().Where(CPP=> CPP.CompanyId == CompanyId).FirstOrDefaultAsync();

                if (comp_profile != null)
                {
                    VATRate = (decimal)comp_profile.Vatrate;
                    //DBC.CreateLog("INFO", "Checking Contract start period Passed the check" + comp_profile.ContractStartDate + "|"+ DateTime.UtcNow);

                    if (comp_profile.ContractStartDate <= DateTime.UtcNow)
                    {

                        // DBC.CreateLog("INFO", "Passed Contract start period Passed the check" + comp_profile.ContractStartDate + "|" + DateTime.UtcNow);

                        CreditLimit = comp_profile.CreditLimit;
                        CreditBalance = comp_profile.CreditBalance;
                        MinimumBalance = comp_profile.MinimumBalance;

                        int TransactionTypeID =await _billing.GetTransactionTypeID("LICENSEFEE");
                        var transaction_type =await db.Set<CompanyTransactionType>()
                                                .Where(TT=> TT.CompanyId == CompanyId && TT.TransactionTypeId == TransactionTypeID).FirstOrDefaultAsync();


                        //check if the contrace (license fee) is to be added in this month statement
                        decimal monthly_payment = 0M;
                        bool havecontract =await _usage.CheckContractFee(CompanyId, comp_profile.ContractStartDate, comp_profile.ContractAnniversary, EndTime, monthly_payment);

                        await  _usage.AddMonthlyItemsToTransactions(CompanyId);
                        //Debit the payment
                        if (havecontract && transaction_type.PaymentMethod == "CREDIT_CARD")
                        {
                           await  _usage.DebitAccount(CompanyId);
                        }
                        else
                        {
                            
                            await SDE.InvoicePaymentAlert(CompanyId, monthly_payment);
                        }

                        //Get the messages transactions from MessageTransaction Tables
                        var transactions = await db.Set<TransactionDetail>()
                                            .Where(TD=> TD.CompanyId == CompanyId && (TD.TransactionDate >= StartTime && TD.TransactionDate <= EndTime)).ToListAsync();

                        foreach (var trans in transactions)
                        {
                            try
                            {
                                if (trans.Drcr == "DR")
                                {
                                    TotalTransactionValue += trans.LineValue;
                                    TotalVatValue += trans.LineVat;
                                    TotalTotal += trans.Total;
                                }
                                if (trans.Drcr == "CR")
                                {
                                    TotalPayments += trans.LineValue;
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }

                        //decimal VatAmount = (TotalTransactionValue * VATRate) / 100;
                        //decimal Total = TotalTransactionValue + VatAmount;

                        //GetUser details and limit
                        var Users = await db.Set<User>()
                                     .Where(U=> U.CompanyId == CompanyId && U.Status != 3)
                                     .Select(U=> new
                                     {
                                         UserRole = string.IsNullOrEmpty(U.UserRole) ? "USER" : U.UserRole
                                     }).ToListAsync();

                        var roles = await DBC.CCRoles(true);

                        int StaffLimit = Convert.ToInt16(DBC.GetPackageItem("USER_LIMIT", CompanyId));
                        int TotalAdmin = Users.Where(w => roles.Contains(w.UserRole.ToUpper())).Select(s => s.UserRole).Count();
                        int TotalStaff = Users.Where(w => w.UserRole.ToUpper() == "USER").Select(s => s.UserRole).Count();
                        int AdminLimit = Convert.ToInt16(DBC.GetPackageItem("ADMIN_USER_LIMIT", CompanyId));

                        //Get storage details
                        int StorageLimit = Convert.ToInt16(DBC.GetPackageItem("MEDIA_STORAGE", CompanyId));
                        var Storage =await db.Set<Assets>().Where(A=> A.CompanyId == CompanyId).ToListAsync();
                        double AssetSize = Storage.Select(s => s.AssetSize).Sum();

                        //Create statement header
                        int tran_header_id =await _usage.AddTransactionHeader(CompanyId, TotalTransactionValue, VATRate, TotalVatValue, TotalTotal, CreditBalance, CreditLimit,
                            AdminLimit, TotalAdmin, StaffLimit, TotalStaff, StorageLimit, AssetSize, StartTime, EndTime);

                        transactions.ForEach(s => s.TransactionHeaderId = tran_header_id);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
