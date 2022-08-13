using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Administrator.Repositories;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Models;
using CrisesControl.Core.Payments;
using CrisesControl.Core.Payments.Repositories;
using CrisesControl.Infrastructure.Context;
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
        private readonly DBCommon DBC;
        public PaymentRepository(CrisesControlContext context, ILogger<PaymentRepository> logger, DBCommon _DBC, IAdminRepository adminRepository)
        {
            this._context = context;
            this._logger = logger;
            this.DBC = _DBC;
            this._adminRepository = adminRepository;
        }
        public async Task<dynamic> GetCompanyByKey(string ActivationKey, int OutUserCompanyId)
        {
            var cp = await _context.Set<Company>().Include(CA => CA.CompanyActivation)
                                    .Where(CA => CA.CompanyActivation.ActivationKey == ActivationKey && CA.CompanyId == OutUserCompanyId && CA.Status == 0
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
        public async Task<bool> OnTrialStatus(string CompanyProfile, bool CurrentTrial)
        {
            if (CompanyProfile == "SUBSCRIBED")
            {
                return false;
            }
            return CompanyProfile == "ON_TRIAL" ? true : CurrentTrial;
        }
        public async Task UpdateCompanyPaymentProfileAsync(int companyId, int currntUserId, string PaymentPeriod, decimal CreditBalance, decimal CreditLimit, decimal MinimumBalance,
           decimal TextUplift, decimal PhoneUplift, decimal EmailUplift, decimal PushUplift, decimal ConfUplift,
           decimal MinimumTextRate, decimal MinimumPhoneRate, decimal MinimumEmailRate, decimal MinimumPushRate, decimal MinimumConfRate,
           string TimeZoneId, DateTimeOffset ContractAnniversary, string AgreementNo, decimal MaxTransactionLimit, DateTimeOffset ContractStartDate,
           string CardType, string CardHolderName, string BillingEmail, string BillingAddress1, string BillingAddress2, string City, string Town, string Postcode,
           string Country)
        {

            var c_pp = await _context.Set<CompanyPaymentProfile>().Where(CPP=> CPP.CompanyId == companyId).FirstOrDefaultAsync();
            if (c_pp != null)
            {

                DateTimeOffset Contractstart = c_pp.ContractStartDate;
                if (ContractStartDate != null)
                {
                    double DaysExceeding = Contractstart.Subtract(ContractStartDate).TotalDays;
                    if (DaysExceeding > 30)
                    {
                        SendEmail SDE = new SendEmail(_context,DBC);
                        await SDE.ContractStartDaysExceeded(companyId, DaysExceeding);
                    }
                }

                if (ContractAnniversary != null)
                    c_pp.ContractAnniversary = ContractAnniversary;

                if (!string.IsNullOrEmpty(PaymentPeriod))
                {
                    c_pp.PaymentPeriod = PaymentPeriod;
                }

                //if(CreditBalance != 9999999999)
                //    c_pp.CreditBalance = CreditBalance;

                if (CreditLimit != 9999999999)
                    c_pp.CreditLimit = CreditLimit;

                if (MinimumBalance != 9999999999)
                {
                    c_pp.MinimumBalance = MinimumBalance;
                    await DBC.SaveParameter(0, "RECHARGE_BALANCE_TRIGGER", MinimumBalance.ToString(), currntUserId, companyId, TimeZoneId);
                }
                c_pp.TextUplift = TextUplift;
                c_pp.PhoneUplift = PhoneUplift;
                c_pp.EmailUplift = EmailUplift;
                c_pp.PushUplift = PushUplift;
                c_pp.ConfUplift = ConfUplift;
                c_pp.MinimumTextRate = MinimumTextRate;
                c_pp.MinimumPhoneRate = MinimumPhoneRate;
                c_pp.MinimumEmailRate = MinimumEmailRate;
                c_pp.MinimumPushRate = MinimumPushRate;
                c_pp.MinimumConfRate = MinimumConfRate;

                if (ContractStartDate != null)
                    c_pp.ContractStartDate = ContractStartDate;

                if (!string.IsNullOrEmpty(CardType))
                    c_pp.CardType = CardType;
                if (!string.IsNullOrEmpty(CardHolderName))
                    c_pp.CardHolderName = CardHolderName;
                if (!string.IsNullOrEmpty(BillingEmail))
                    c_pp.BillingEmail = BillingEmail;
                if (!string.IsNullOrEmpty(BillingAddress1))
                    c_pp.BillingAddress1 = BillingAddress1;
                if (!string.IsNullOrEmpty(BillingAddress2))
                    c_pp.BillingAddress2 = BillingAddress2;
                if (!string.IsNullOrEmpty(City))
                    c_pp.City = City;
                if (!string.IsNullOrEmpty(Town))
                    c_pp.Town = Town;
                if (!string.IsNullOrEmpty(Postcode))
                    c_pp.Postcode = Postcode;
                if (!string.IsNullOrEmpty(Country))
                    c_pp.Country = Country;

                if (!string.IsNullOrEmpty(AgreementNo))
                    c_pp.AgreementNo = AgreementNo;

                if (MaxTransactionLimit > 0)
                    c_pp.MaxTransactionLimit = MaxTransactionLimit;
                c_pp.UpdatedBy = currntUserId;
                c_pp.UpdatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CompanyPackage> GetCompanyPackageItems(int OutUserCompanyId)
        {
            try
            {
                var CompanyItem = await  _context.Set<Company>().Include(x=>x.PackagePlan)
                                   .Where(C=> C.CompanyId == OutUserCompanyId && C.PackagePlan.PackagePlanId == C.PackagePlanId)
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

        public async Task<List<PackageItems>> GetPackageAddons(int OutUserCompanyId, bool ShowAll = false)
        {
            try
            {
                var packageitemlist =await _context.Set<PackageItems>().FromSqlRaw("Pro_Payments_GetPackageAddons @OutUserCompanyId", OutUserCompanyId).ToListAsync();
                if (!ShowAll)
                    packageitemlist = packageitemlist.Where(s => s.Status == 0).ToList();


                return packageitemlist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<CompanyPaymentProfile> UpdateCompanyPaymentProfile(UpdateCompanyPaymentProfileModel IP,int CurrentUserId, int OutUserCompanyId, string TimeZoneId)
        {
            try
            {
               

               await UpdateCompanyPaymentProfileAsync(OutUserCompanyId, CurrentUserId, IP.PaymentPeriod, IP.CreditBalance, IP.CreditLimit, IP.MinimumBalance,
                    IP.TextUplift, IP.PhoneUplift, IP.EmailUplift, IP.PushUplift, IP.ConfUplift,
                    IP.MinimumTextRate, IP.MinimumPhoneRate, IP.MinimumEmailRate, IP.MinimumPushRate, IP.MinimumConfRate,
                    TimeZoneId, IP.ContractAnniversary, IP.AgreementNo, IP.MaxTransactionLimit, IP.ContractStartDate,
                    IP.CardType, IP.CardHolderName, IP.BillingEmail, IP.BillingAddress1, IP.BillingAddress2, IP.City, IP.Town, IP.Postcode, IP.Country);

                var trans_id = await  _context.Set<CompanyTransactionType>().Include(x=>x.TransactionType)
                                 .Where(CT=> CT.TransactionType.TransactionCode == IP.TransactionCode && CT.CompanyId == OutUserCompanyId
                                ).FirstOrDefaultAsync();

                int transaction_id = 0;
                if (trans_id != null)
                    transaction_id = trans_id.CompanyTranscationTypeId;

                await UpdateCompanyTranscationType(OutUserCompanyId, CurrentUserId, TimeZoneId, trans_id.TransactionTypeId, IP.ContractValue, transaction_id, IP.PaymentPeriod, null, IP.PaymentMethod);

                if (!string.IsNullOrEmpty(IP.CompanyProfile))
                {
                    var comp = await _context.Set<Company>().Where(C=> C.CompanyId == OutUserCompanyId).FirstOrDefaultAsync();
                    if (comp != null)
                    {
                        comp.CompanyProfile = IP.CompanyProfile;
                        if (IP.OnTrial)
                        {
                            comp.OnTrial = IP.OnTrial;
                        }
                        else
                        {
                            comp.OnTrial = DBC.OnTrialStatus(IP.CompanyProfile, comp.OnTrial);
                        }

                       await _context.SaveChangesAsync();
                    }
                }

                if (IP.AgreementRegistered)
                {
                    SendEmail SDE = new SendEmail(_context,DBC);
                    SDE.WorldPayAgreementSubscribe(OutUserCompanyId, IP.AgreementNo);
                }

                var Data = await _context.Set<CompanyPaymentProfile>().Where(CPP=> CPP.CompanyId == OutUserCompanyId).FirstOrDefaultAsync();
                if (Data != null)
                {
                    if (IP.AgreementRegistered)
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
        public async Task<int> UpdateCompanyTranscationType(int companyId, int currntUserId, string TimeZoneId, int TransactionTypeID, decimal TransactionRate,
          int CompnayTranscationTypeId = 0, string PaymentPeriod = "MONTHLY", DateTimeOffset? NextRunDate = null, string PaymentMethod = "INVOICE")
        {
            int CTTId = 0;
            if (CompnayTranscationTypeId == 0)
            {
                CompanyTransactionType transaction = new CompanyTransactionType();
                if (TransactionTypeID > 0)
                    transaction.TransactionTypeId = TransactionTypeID;
                transaction.TransactionRate = TransactionRate;
                transaction.CompanyId = companyId;
                transaction.PaymentPeriod = PaymentPeriod;
                if (NextRunDate.HasValue)
                {
                    transaction.NextRunDate = (DateTimeOffset)NextRunDate;
                }
                else
                {
                    transaction.NextRunDate = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                }
                transaction.CreatedBy = currntUserId;
                transaction.CreatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                transaction.UpdatedBy = currntUserId;
                transaction.UpdatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);

                if (!string.IsNullOrEmpty(PaymentMethod) && PaymentMethod != "UNKNOWN")
                    transaction.PaymentMethod = PaymentMethod;

                await _context.AddAsync(transaction);
                await _context.SaveChangesAsync();
                CTTId = transaction.CompanyTranscationTypeId;
            }
            else
            {
                var newCompanyTranscationType = await _context.Set<CompanyTransactionType>()
                                                 .Where(CTT=> CTT.CompanyTranscationTypeId == CompnayTranscationTypeId && CTT.CompanyId == companyId
                                                 ).FirstOrDefaultAsync();
                if (newCompanyTranscationType != null)
                {
                    if (TransactionTypeID > 0)
                        newCompanyTranscationType.TransactionTypeId = TransactionTypeID;

                    newCompanyTranscationType.TransactionRate = TransactionRate;
                    newCompanyTranscationType.PaymentPeriod = PaymentPeriod;
                    newCompanyTranscationType.PaymentMethod = PaymentMethod;

                    if (NextRunDate.HasValue)
                    {
                        newCompanyTranscationType.NextRunDate = (DateTimeOffset)NextRunDate;
                    }
                    newCompanyTranscationType.UpdatedBy = currntUserId;
                    newCompanyTranscationType.UpdatedOn = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                     _context.Update(newCompanyTranscationType);
                    await _context.SaveChangesAsync();
                    CTTId = newCompanyTranscationType.CompanyTranscationTypeId;
                }
            }
            return CTTId;
        }

        public async Task<bool> UpgradePackage(UpdateCompanyPaymentProfileModel IP,int CurrentUserId, int OutUserCompanyId, string TimeZoneId)
        {
            try
            {
               
                var cp =  await _context.Set<CompanyPaymentProfile>().Include(CP=>CP.Company)
                         .Where(C=> C.CompanyId == OutUserCompanyId
                          ).FirstOrDefaultAsync();
                if (cp != null)
                {
                    DateTimeOffset dtNow = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);

                    cp.Company.CompanyProfile = IP.CompanyProfile;
                    cp.Company.OnTrial = DBC.OnTrialStatus(IP.CompanyProfile, false);
                    cp.Company.AnniversaryDate = DBC.GetDateTimeOffset(DateTime.Now.AddYears(1).AddDays(-1), TimeZoneId);
                    cp.AgreementNo = IP.AgreementNo;
                    cp.BillingAddress1 = IP.BillingAddress1;
                    cp.BillingAddress2 = IP.BillingAddress2;
                    cp.BillingEmail = IP.BillingEmail;
                    cp.Town = IP.Town;
                    cp.City = IP.City;
                    cp.Country = IP.Country;
                    cp.Ipaddress = IP.IPAddress;
                    cp.PaymentPeriod = IP.PaymentPeriod;
                    cp.ContractAnniversary = DBC.GetDateTimeOffset(DateTime.Now.AddYears(1).AddDays(-1), TimeZoneId);
                    cp.ContractStartDate = dtNow;
                    cp.CardFailed = false;
                    cp.CardHolderName = IP.CardHolderName;
                    cp.CardType = IP.CardType;
                    cp.MaxTransactionLimit = IP.ContractValue;
                    cp.UpdatedOn = dtNow;
                    cp.UpdatedBy = CurrentUserId;
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
                        

                       await _adminRepository.AddTransaction(utd, CurrentUserId, OutUserCompanyId, TimeZoneId);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    try
                    {
                        var trans_id = await _context.Set<CompanyTransactionType>().Include(x=>x.TransactionType)
                                       .Where(CT=> CT.TransactionType.TransactionCode == IP.TransactionCode && CT.CompanyId == OutUserCompanyId
                                        ).FirstOrDefaultAsync();

                        int transaction_id = 0;
                        if (trans_id != null)
                        {
                            transaction_id = trans_id.CompanyTranscationTypeId;
                            await UpdateCompanyTranscationType(OutUserCompanyId, CurrentUserId, TimeZoneId, trans_id.TransactionTypeId, IP.ContractValue, transaction_id, IP.PaymentPeriod, null, IP.PaymentMethod);
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
                            await UpdateCompanyTranscationType(OutUserCompanyId, CurrentUserId, TimeZoneId, mod.TransactionTypeId, mod.Rate, 0, IP.PaymentPeriod, null, IP.PaymentMethod);

                            //Enable the module for the company
                            var module = (from CPF in _context.Set<CompanyPackageFeature>()
                                          join CF in _context.Set<ChargeableFeature>() on CPF.SecurityObjectId equals CF.SecurityObjectId
                                          where CPF.CompanyId == OutUserCompanyId && CF.TransactionTypeId == mod.TransactionTypeId
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
                        SendEmail SDE = new SendEmail(_context,DBC);
                        SDE.WorldPayAgreementSubscribe(OutUserCompanyId, IP.AgreementNo);
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
    }
}
