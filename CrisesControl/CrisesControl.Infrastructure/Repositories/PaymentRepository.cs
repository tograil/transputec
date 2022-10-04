using CrisesControl.Core.Administrator;
using CrisesControl.Core.Administrator.Repositories;
using CrisesControl.Core.Companies;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Models;
using CrisesControl.Core.Payments;
using CrisesControl.Core.Payments.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class PaymentRepository: IPaymentRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<PaymentRepository> _logger;
        private readonly IAdminRepository _adminRepository;
        private readonly IDBCommonRepository DBC;
        private readonly ISenderEmailService _SDE;
        public PaymentRepository(CrisesControlContext context, ILogger<PaymentRepository> logger, IDBCommonRepository _DBC, IAdminRepository adminRepository, ISenderEmailService SDE)
        {
            this._context = context;
            this._logger = logger;
            this.DBC = _DBC;
            this._adminRepository = adminRepository;
            this._SDE = SDE;
        }
        public async Task<dynamic> GetCompanyByKey(string activationKey, int outUserCompanyId)
        {
            var cp = await _context.Set<Company>().Include(CA => CA.CompanyActivation)
                                    .Where(CA => CA.CompanyActivation.ActivationKey == activationKey && CA.CompanyId == outUserCompanyId && CA.Status == 0
                                    ).FirstOrDefaultAsync();
       
            return cp;
        }
        public async Task<int> UpgradeByKey(Company company)
        {
           
                _context.Update(company);
              await  _context.SaveChangesAsync();
            _logger.LogInformation("update payment for " + company.CompanyActivation.ActivationKey);
            return company.CompanyId;
              
                
            
        }
        public async Task<bool> OnTrialStatus(string companyProfile, bool currentTrial)
        {
            if (companyProfile == "SUBSCRIBED")
            {
                return false;
            }
            return companyProfile == "ON_TRIAL" ? true : currentTrial;
        }
        public async Task UpdateCompanyPaymentProfileAsync(int companyId, int currntUserId, string paymentPeriod, decimal creditBalance, decimal creditLimit, decimal minimumBalance,
           decimal textUplift, decimal phoneUplift, decimal emailUplift, decimal pushUplift, decimal confUplift,
           decimal minimumTextRate, decimal minimumPhoneRate, decimal minimumEmailRate, decimal minimumPushRate, decimal minimumConfRate,
           string timeZoneId, DateTimeOffset contractAnniversary, string agreementNo, decimal maxTransactionLimit, DateTimeOffset contractStartDate,
           string cardType, string cardHolderName, string billingEmail, string billingAddress1, string billingAddress2, string city, string town, string postcode,
           string country)
        {

            var c_pp = await _context.Set<CompanyPaymentProfile>().Where(CPP=> CPP.CompanyId == companyId).FirstOrDefaultAsync();
            if (c_pp != null)
            {

                DateTimeOffset Contractstart = c_pp.ContractStartDate;
                if (contractStartDate != null)
                {
                    double DaysExceeding = Contractstart.Subtract(contractStartDate).TotalDays;
                    if (DaysExceeding > 30)
                    {
                        
                        await _SDE.ContractStartDaysExceeded(companyId, DaysExceeding);
                    }
                }

                if (contractAnniversary != null)
                    c_pp.ContractAnniversary = contractAnniversary;

                if (!string.IsNullOrEmpty(paymentPeriod))
                {
                    c_pp.PaymentPeriod = paymentPeriod;
                }

                //if(CreditBalance != 9999999999)
                //    c_pp.CreditBalance = CreditBalance;

                if (creditLimit != 9999999999)
                    c_pp.CreditLimit = creditLimit;

                if (minimumBalance != 9999999999)
                {
                    c_pp.MinimumBalance = minimumBalance;
                    await DBC.SaveParameter(0, "RECHARGE_BALANCE_TRIGGER", minimumBalance.ToString(), currntUserId, companyId, timeZoneId);
                }
                c_pp.TextUplift = textUplift;
                c_pp.PhoneUplift = phoneUplift;
                c_pp.EmailUplift = emailUplift;
                c_pp.PushUplift = pushUplift;
                c_pp.ConfUplift = confUplift;
                c_pp.MinimumTextRate = minimumTextRate;
                c_pp.MinimumPhoneRate = minimumPhoneRate;
                c_pp.MinimumEmailRate = minimumEmailRate;
                c_pp.MinimumPushRate = minimumPushRate;
                c_pp.MinimumConfRate = minimumConfRate;

                if (contractStartDate != null)
                    c_pp.ContractStartDate = contractStartDate;

                if (!string.IsNullOrEmpty(cardType))
                    c_pp.CardType = cardType;
                if (!string.IsNullOrEmpty(cardHolderName))
                    c_pp.CardHolderName = cardHolderName;
                if (!string.IsNullOrEmpty(billingEmail))
                    c_pp.BillingEmail = billingEmail;
                if (!string.IsNullOrEmpty(billingAddress1))
                    c_pp.BillingAddress1 = billingAddress1;
                if (!string.IsNullOrEmpty(billingAddress2))
                    c_pp.BillingAddress2 = billingAddress2;
                if (!string.IsNullOrEmpty(city))
                    c_pp.City = city;
                if (!string.IsNullOrEmpty(town))
                    c_pp.Town = town;
                if (!string.IsNullOrEmpty(postcode))
                    c_pp.Postcode = postcode;
                if (!string.IsNullOrEmpty(country))
                    c_pp.Country = country;

                if (!string.IsNullOrEmpty(agreementNo))
                    c_pp.AgreementNo = agreementNo;

                if (maxTransactionLimit > 0)
                    c_pp.MaxTransactionLimit = maxTransactionLimit;
                c_pp.UpdatedBy = currntUserId;
                c_pp.UpdatedOn =await DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CompanyPackage> GetCompanyPackageItems(int outUserCompanyId)
        {
            try
            {
                var CompanyItem = await  _context.Set<Company>().Include(x=>x.PackagePlan)
                                   .Where(C=> C.CompanyId == outUserCompanyId && C.PackagePlan.PackagePlanId == C.PackagePlanId)
                                   .Select( P => new CompanyPackage
                                   {
                                       PlanName = P.PackagePlan.PlanName,
                                       PlanDescription = P.PackagePlan.PlanDescription,
                                       PackageItems = _context.Set<CompanyPackageItem>().Include(LPI=>LPI.LibPackageItem)
                                                       .Where(PI=> PI.CompanyId == P.CompanyId && PI.Status == 1 && PI.LibPackageItem.PackagePlanId == P.PackagePlanId)
                                                       .Select(PI => new CompanyPackageItems
                                                          {
                                                            CompanyPackageItemId= PI.CompanyPackageItemId,
                                                            ItemCode= PI.ItemCode,
                                                            ItemName= PI.LibPackageItem.ItemName,
                                                            ItemDescription= PI.LibPackageItem.ItemDescription,
                                                            ItemValue= PI.ItemValue,
                                                            Status= PI.Status,
                                                           }).Distinct().FirstOrDefault(),
                                       TransactionRates =  _context.Set<CompanyTransactionType>().Include(x=>x.TransactionType)
                                                           .Select(TT=> new TransactionRates
                                                           {
                                                               TransactionCode=  TT.TransactionType.TransactionCode,
                                                               TransactionDescription= TT.TransactionType.TransactionDescription,
                                                               Rate = (TT.TransactionRate == null ? TT.TransactionType.Rate : TT.TransactionRate)
                                                           }).FirstOrDefault()
                                   }).FirstOrDefaultAsync();

                if (CompanyItem != null)
                {
                    return CompanyItem;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<PackageItems>> GetPackageAddons(int outUserCompanyId, bool showAll = false)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyId", outUserCompanyId);
                var packageitemlist =await _context.Set<PackageItems>().FromSqlRaw("exec [Pro_Admin_GetPackageAddons] @CompanyId", pCompanyID).ToListAsync();
                if (!showAll)
                    packageitemlist = packageitemlist.Where(s => s.Status == 0).ToList();


                return packageitemlist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<CompanyPaymentProfile> UpdateCompanyPaymentProfile(UpdateCompanyPaymentProfileModel ip,int currentUserId, int outUserCompanyId, string timeZoneId)
        {
            try
            {
               

               await UpdateCompanyPaymentProfileAsync(outUserCompanyId, currentUserId, ip.PaymentPeriod, ip.CreditBalance, ip.CreditLimit, ip.MinimumBalance,
                    ip.TextUplift, ip.PhoneUplift, ip.EmailUplift, ip.PushUplift, ip.ConfUplift,
                    ip.MinimumTextRate, ip.MinimumPhoneRate, ip.MinimumEmailRate, ip.MinimumPushRate, ip.MinimumConfRate,
                    timeZoneId, ip.ContractAnniversary, ip.AgreementNo, ip.MaxTransactionLimit, ip.ContractStartDate,
                    ip.CardType, ip.CardHolderName, ip.BillingEmail, ip.BillingAddress1, ip.BillingAddress2, ip.City, ip.Town, ip.Postcode, ip.Country);

                var trans_id = await  _context.Set<CompanyTransactionType>().Include(x=>x.TransactionType)
                                 .Where(CT=> CT.TransactionType.TransactionCode == ip.TransactionCode && CT.CompanyId == outUserCompanyId
                                ).FirstOrDefaultAsync();

                int transaction_id = 0;
                if (trans_id != null)
                    transaction_id = trans_id.CompanyTranscationTypeId;

                await UpdateCompanyTranscationType(outUserCompanyId, currentUserId, timeZoneId, trans_id.TransactionTypeId, ip.ContractValue, transaction_id, ip.PaymentPeriod, null, ip.PaymentMethod);

                if (!string.IsNullOrEmpty(ip.CompanyProfile))
                {
                    var comp = await _context.Set<Company>().Where(C=> C.CompanyId == outUserCompanyId).FirstOrDefaultAsync();
                    if (comp != null)
                    {
                        comp.CompanyProfile = ip.CompanyProfile;
                        if (ip.OnTrial)
                        {
                            comp.OnTrial = ip.OnTrial;
                        }
                        else
                        {
                            comp.OnTrial =await DBC.OnTrialStatus(ip.CompanyProfile, comp.OnTrial);
                        }

                       await _context.SaveChangesAsync();
                    }
                }

                if (ip.AgreementRegistered)
                {
                    
                   await _SDE.WorldPayAgreementSubscribe(outUserCompanyId, ip.AgreementNo);
                }

                var Data = await _context.Set<CompanyPaymentProfile>().Where(CPP=> CPP.CompanyId == outUserCompanyId).FirstOrDefaultAsync();
                if (Data != null)
                {
                    if (ip.AgreementRegistered)
                    {
                        Data.CardFailed = false;
                        _context.Update(Data);
                        await _context.SaveChangesAsync();
                    }
                    return Data;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<int> UpdateCompanyTranscationType(int companyId, int currntUserId, string timeZoneId, int transactionTypeID, decimal transactionRate,
          int compnayTranscationTypeId = 0, string paymentPeriod = "MONTHLY", DateTimeOffset? nextRunDate = null, string paymentMethod = "INVOICE")
        {
            int CTTId = 0;
            if (compnayTranscationTypeId == 0)
            {
                CompanyTransactionType transaction = new CompanyTransactionType();
                if (transactionTypeID > 0)
                    transaction.TransactionTypeId = transactionTypeID;
                transaction.TransactionRate = transactionRate;
                transaction.CompanyId = companyId;
                transaction.PaymentPeriod = paymentPeriod;
                if (nextRunDate.HasValue)
                {
                    transaction.NextRunDate = (DateTimeOffset)nextRunDate;
                }
                else
                {
                    transaction.NextRunDate =await DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                }
                transaction.CreatedBy = currntUserId;
                transaction.CreatedOn =await DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                transaction.UpdatedBy = currntUserId;
                transaction.UpdatedOn =await DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);

                if (!string.IsNullOrEmpty(paymentMethod) && paymentMethod != "UNKNOWN")
                    transaction.PaymentMethod = paymentMethod;

                await _context.AddAsync(transaction);
                await _context.SaveChangesAsync();
                CTTId = transaction.CompanyTranscationTypeId;
            }
            else
            {
                var newCompanyTranscationType = await _context.Set<CompanyTransactionType>()
                                                 .Where(CTT=> CTT.CompanyTranscationTypeId == compnayTranscationTypeId && CTT.CompanyId == companyId
                                                 ).FirstOrDefaultAsync();
                if (newCompanyTranscationType != null)
                {
                    if (transactionTypeID > 0)
                        newCompanyTranscationType.TransactionTypeId = transactionTypeID;

                    newCompanyTranscationType.TransactionRate = transactionRate;
                    newCompanyTranscationType.PaymentPeriod = paymentPeriod;
                    newCompanyTranscationType.PaymentMethod = paymentMethod;

                    if (nextRunDate.HasValue)
                    {
                        newCompanyTranscationType.NextRunDate = (DateTimeOffset)nextRunDate;
                    }
                    newCompanyTranscationType.UpdatedBy = currntUserId;
                    newCompanyTranscationType.UpdatedOn =await DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                     _context.Update(newCompanyTranscationType);
                    await _context.SaveChangesAsync();
                    CTTId = newCompanyTranscationType.CompanyTranscationTypeId;
                }
            }
            return CTTId;
        }

        public async Task<bool> UpgradePackage(UpdateCompanyPaymentProfileModel IP,int currentUserId, int outUserCompanyId, string timeZoneId)
        {
            try
            {
               
                var cp =  await _context.Set<CompanyPaymentProfile>().Include(CP=>CP.Company)
                         .Where(C=> C.CompanyId == outUserCompanyId
                          ).FirstOrDefaultAsync();
                if (cp != null)
                {
                    DateTimeOffset dtNow =await DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);

                    cp.Company.CompanyProfile = IP.CompanyProfile;
                    cp.Company.OnTrial =await DBC.OnTrialStatus(IP.CompanyProfile, false);
                    cp.Company.AnniversaryDate =await DBC.GetDateTimeOffset(DateTime.Now.AddYears(1).AddDays(-1), timeZoneId);
                    cp.AgreementNo = IP.AgreementNo;
                    cp.BillingAddress1 = IP.BillingAddress1;
                    cp.BillingAddress2 = IP.BillingAddress2;
                    cp.BillingEmail = IP.BillingEmail;
                    cp.Town = IP.Town;
                    cp.City = IP.City;
                    cp.Country = IP.Country;
                    cp.Ipaddress = IP.IPAddress;
                    cp.PaymentPeriod = IP.PaymentPeriod;
                    cp.ContractAnniversary =await DBC.GetDateTimeOffset(DateTime.Now.AddYears(1).AddDays(-1), timeZoneId);
                    cp.ContractStartDate = dtNow;
                    cp.CardFailed = false;
                    cp.CardHolderName = IP.CardHolderName;
                    cp.CardType = IP.CardType;
                    cp.MaxTransactionLimit = IP.ContractValue;
                    cp.UpdatedOn = dtNow;
                    cp.UpdatedBy = currentUserId;
                    _context.Update(cp);
                   await _context.SaveChangesAsync();


                    //Add Topup transaction
                    try
                    {
                        
                        int TopupTypeId = _context.Set<TransactionType>().Where(w => w.TransactionCode == "TOPUP").Select(s => s.TransactionTypeId).FirstOrDefault();
                        decimal VatRate = (decimal)cp.Vatrate;
                        VatRate = VatRate / 100;

                        decimal LineVat = IP.CreditBalance * VatRate;
                        decimal LineValue = IP.CreditBalance - LineVat;

                        UpdateTransactionDetailsModel utd = new UpdateTransactionDetailsModel();
                        utd.TransactionTypeId = TopupTypeId;
                        utd.TransactionRate = IP.CreditBalance;
                        utd.MinimumPrice = IP.CreditBalance;
                        utd.Quantity = 1;
                        utd.Cost = IP.CreditBalance;
                        utd.LineValue = LineValue;
                        utd.Vat = LineVat;
                        utd.Total = IP.CreditBalance;
                        utd.TransactionDate = dtNow;
                        utd.TransactionReference = IP.WTransactionID;
                        utd.TransactionStatus = 1;
                        utd.DRCR = "CR";
                        utd.PaymentMethod = IP.PaymentMethod;
                        

                       await _adminRepository.AddTransaction(utd, currentUserId, outUserCompanyId, timeZoneId);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    try
                    {
                        var trans_id = await _context.Set<CompanyTransactionType>().Include(x=>x.TransactionType)
                                       .Where(CT=> CT.TransactionType.TransactionCode == IP.TransactionCode && CT.CompanyId == outUserCompanyId
                                        ).FirstOrDefaultAsync();

                        int transaction_id = 0;
                        if (trans_id != null)
                        {
                            transaction_id = trans_id.CompanyTranscationTypeId;
                            await UpdateCompanyTranscationType(outUserCompanyId, currentUserId, timeZoneId, trans_id.TransactionTypeId, IP.ContractValue, transaction_id, IP.PaymentPeriod, null, IP.PaymentMethod);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }


                    if (IP.Modules != null)
                    {
                        var tt = _context.Set<TransactionType>().Where(w => IP.Modules.Contains(w.TransactionTypeId)).ToList();
                        foreach (var mod in tt)
                        {
                            //Add the company transaction type entry
                            await UpdateCompanyTranscationType(outUserCompanyId, currentUserId, timeZoneId, mod.TransactionTypeId, mod.Rate, 0, IP.PaymentPeriod, null, IP.PaymentMethod);

                            //Enable the module for the company
                            var module = (from CPF in _context.Set<CompanyPackageFeature>()
                                          join CF in _context.Set<ChargeableFeature>() on CPF.SecurityObjectId equals CF.SecurityObjectId
                                          where CPF.CompanyId == outUserCompanyId && CF.TransactionTypeId == mod.TransactionTypeId
                                          select CPF).FirstOrDefault();
                            if (module != null)
                            {
                                module.IsPaid = true;
                                module.Status = 1;
                               await _context.SaveChangesAsync();
                            }
                        }
                    }

                    if (IP.AgreementRegistered)
                    {
                      
                       await _SDE.WorldPayAgreementSubscribe(outUserCompanyId, IP.AgreementNo);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> AddRemoveModule(int companyID, int moduleID, string actionVal)
        {
            try
            {

                var pCompanyID = new SqlParameter("@CompanyID", companyID);
                var pModuleID = new SqlParameter("@ModuleID", moduleID);
                var pActionVal = new SqlParameter("@Action", actionVal);

                await  _context.Database.ExecuteSqlRawAsync("Pro_Admin_Add_Remove_Module @CompanyID, @ModuleID, @Action", pCompanyID, pModuleID, pActionVal);
                return true;
            }
            catch (Exception ex)
            {
               return false;
            }
        }
    }
}
