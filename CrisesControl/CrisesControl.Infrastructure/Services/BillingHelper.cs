
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Models;
using CrisesControl.Core.Companies;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrisesControl.Core.DBCommon.Repositories;

namespace CrisesControl.Infrastructure.Services
{
    public class BillingHelper
    {
        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDBCommonRepository DBC;
        private readonly ISenderEmailService _SDE;

        public int active_users = 0;
        public int admin_key_count = 0;
        public int staff_count = 0;
        public bool count_query = false;
        public string current_user_role = string.Empty;
        public BillingHelper(CrisesControlContext db, IDBCommonRepository _DBC, ISenderEmailService SDE)
        {
            _httpContextAccessor = new HttpContextAccessor();
            _context = db;
            DBC = _DBC;
            _SDE = SDE;
        }
        public async Task<int> GetTransactionTypeID(string TransactionCode)
        {
            int TextTransacTypeId = await _context.Set<TransactionType>().Where(w => w.TransactionCode == TransactionCode).Select(s => s.TransactionTypeId).FirstOrDefaultAsync();
            return TextTransacTypeId;
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
                        await _context.AddAsync(NewTransactionDetails);
                        await _context.SaveChangesAsync();
                        TDId = NewTransactionDetails.TransactionDetailsId;

                    }
                    else
                    {
                        var newTransactionDetails = await _context.Set<TransactionDetail>().Where(TD => TD.TransactionDetailsId == TransactionDetailsId).FirstOrDefaultAsync();
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
                            newTransactionDetails.UpdateOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                            newTransactionDetails.Drcr = TrType;
                            _context.Update(newTransactionDetails);
                            await _context.SaveChangesAsync();
                            TDId = newTransactionDetails.TransactionDetailsId;
                        }
                    }
                }
                return TDId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
         }
        public async Task<int> AddMonthTransaction(int TransactionID, int CompanyID, int UserID, string UserRole, decimal TransactionValue, int TransactionTypeID, DateTimeOffset? TransactionDate = null)
        {
            try
            {

                MonthlyTransactionItem MTI = new MonthlyTransactionItem();
                MTI.ItemValue = TransactionValue;
                MTI.CompanyId = CompanyID;
                MTI.TransactionDate = (DateTimeOffset)(TransactionDate.HasValue ? TransactionDate : DateTime.Now);
                MTI.TransactionTypeId = TransactionTypeID;
                MTI.UserId = UserID;
                MTI.UserRole = UserRole;
               await  _context.AddAsync(MTI);
                await _context.SaveChangesAsync();
                return MTI.TransactionId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }

        public async Task UpdateThisMonthOnly(int TransactionID, bool Switch, bool SwitchFlag = true)
        {
            if (Switch == true && TransactionID > 0)
            {
                var transaction = await  _context.Set<MonthlyTransactionItem>().Where(MI=> MI.TransactionId == TransactionID).FirstOrDefaultAsync();
                if (transaction != null)
                {
                    transaction.ThisMonthOnly = SwitchFlag;
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
            }
        }
        public DateTimeOffset GetNextRunDate(DateTimeOffset DateNow, string Period = "MONTHLY", int Adjustment = -1)
        {
            DateTimeOffset returndt = DateNow;
            DateTime firstDay = new DateTime(DateNow.Year, DateNow.Month, 1);
            if (Period == "MONTHLY")
            {
                DateTimeOffset lastDayOfMonth = firstDay.AddMonths(1).AddDays(Adjustment);
                returndt = lastDayOfMonth;
            }
            else
            {
                returndt = firstDay.AddYears(1).AddDays(Adjustment);

            }
            return returndt;
        }
        public async Task GetCompanyUserCount(int outUserCompanyId, int userID, int adminCount,int keyHolderCount, int staffCount, int activeUserCount, int pendingUserCount)
        {

            List<BillingStats> result = await GetBillingStats(outUserCompanyId, userID);
            var roles =await DBC.CCRoles();

            adminCount = result.Where(w => roles.Contains(w.PropertyName.ToUpper())).Sum(su => su.TotalCount);
            keyHolderCount = result.Where(w => w.PropertyName.ToUpper() == "KEYHOLDER").Select(s => s.TotalCount).FirstOrDefault();
            staffCount = result.Where(w => w.PropertyName.ToUpper() == "USER").Select(s => s.TotalCount).FirstOrDefault();
            pendingUserCount = result.Where(w => w.PropertyName == "PENDING_USER").Select(s => s.TotalCount).FirstOrDefault();
            activeUserCount = result.Where(w => w.PropertyName == "ACTIVE_USER").Select(s => s.TotalCount).FirstOrDefault();
        }

        public async Task<List<BillingStats>> GetBillingStats(int companyID, int userID)
        {
            try
            {

                
                    var pCompanyID = new SqlParameter("@CompanyID", companyID);
                    var pUserID = new SqlParameter("@UserID", userID);

                    var result = await _context.Set<BillingStats>().FromSqlRaw("exec Pro_Get_Billing_Stats @CompanyID, @UserID", pCompanyID, pUserID).ToListAsync();

                    return result;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AddTransaction(UpdateTransactionDetailsModel IP, int CurrentUserId, int CompanyId, string TimeZoneId)
        {
            try
            {
                var ttype = await _context.Set<TransactionType>().Where(TT => TT.TransactionTypeId == IP.TransactionTypeId).FirstOrDefaultAsync();
                if (ttype != null)
                {

                    //Make a transaction now
                    int Transid = await UpdateTransactionDetails(0, CompanyId, IP.TransactionTypeId, IP.TransactionRate, IP.MinimumPrice, IP.Quantity,
                        IP.Cost, IP.LineValue, IP.Vat, IP.Total, 0, IP.TransactionDate, CurrentUserId, IP.TransactionReference, TimeZoneId,
                        IP.TransactionStatus, 0, IP.DRCR);

                    if (Transid > 0)
                    {
                        var comp_pp = await _context.Set<CompanyPaymentProfile>().Where(CPP => CPP.CompanyId == CompanyId).FirstOrDefaultAsync();
                        if (comp_pp != null)
                        {

                            //Handle the TOPUP Transaction
                            if (ttype.TransactionCode == "TOPUP")
                            {
                                decimal newBalance = comp_pp.CreditBalance + IP.Total;
                                comp_pp.CreditBalance = newBalance;
                                comp_pp.LastCreditDate = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                                comp_pp.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                                comp_pp.UpdatedBy = CurrentUserId;
                                _context.Update(comp_pp);
                                await _context.SaveChangesAsync();

                                await DBC.GetSetCompanyComms(CompanyId);

                                if (IP.PaymentMethod == "SELFTOPUP")
                                {

                                    _SDE.SendPaymentTransactionAlert(CompanyId, IP.Total, TimeZoneId);
                                }
                            }

                            //Handle the Storage Transaction
                            if (ttype.TransactionCode == "STORAGE")
                            {
                                comp_pp.StorageLimit = comp_pp.StorageLimit + IP.Storage;
                                comp_pp.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                                comp_pp.UpdatedBy = CurrentUserId;
                                _context.Update(comp_pp);
                                await _context.SaveChangesAsync();
                                return Transid = comp_pp.CompanyId;
                            }
                        }
                        //ResultDTO.ErrorId = 0;
                        //ResultDTO.Message = "Transaction entry is successful";
                        return Transid;
                    }
                    //else
                    //{
                    //    ResultDTO.ErrorId = 100;
                    //    ResultDTO.Message = "Transaction was not successfull";
                    //}
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
