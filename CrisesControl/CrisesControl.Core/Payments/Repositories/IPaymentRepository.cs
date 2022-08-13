using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Payments.Repositories
{
    public interface IPaymentRepository
    {
        Task<dynamic> GetCompanyByKey(string ActivationKey, int OutUserCompanyId);
        //Task<CompanyActivation> GetCompanyByKey(string ActivationKey, int OutUserCompanyId);
        Task<bool> OnTrialStatus(string CompanyProfile, bool CurrentTrial);
        Task<int> UpgradeByKey(Company company);
        Task<bool> UpgradePackage(UpdateCompanyPaymentProfileModel IP, int CurrentUserId, int OutUserCompanyId, string TimeZoneId);
        Task<int> UpdateCompanyTranscationType(int companyId, int currntUserId, string TimeZoneId, int TransactionTypeID, decimal TransactionRate,
          int CompnayTranscationTypeId = 0, string PaymentPeriod = "MONTHLY", DateTimeOffset? NextRunDate = null, string PaymentMethod = "INVOICE");
        Task<CompanyPaymentProfile> UpdateCompanyPaymentProfile(UpdateCompanyPaymentProfileModel IP, int CurrentUserId, int OutUserCompanyId, string TimeZoneId);
        Task<List<PackageItems>> GetPackageAddons(int OutUserCompanyId, bool ShowAll = false);
        Task<CompanyPackage> GetCompanyPackageItems(int OutUserCompanyId);
        Task UpdateCompanyPaymentProfileAsync(int companyId, int currntUserId, string PaymentPeriod, decimal CreditBalance, decimal CreditLimit, decimal MinimumBalance,
           decimal TextUplift, decimal PhoneUplift, decimal EmailUplift, decimal PushUplift, decimal ConfUplift,
           decimal MinimumTextRate, decimal MinimumPhoneRate, decimal MinimumEmailRate, decimal MinimumPushRate, decimal MinimumConfRate,
           string TimeZoneId, DateTimeOffset ContractAnniversary, string AgreementNo, decimal MaxTransactionLimit, DateTimeOffset ContractStartDate,
           string CardType, string CardHolderName, string BillingEmail, string BillingAddress1, string BillingAddress2, string City, string Town, string Postcode,
           string Country);
        Task<bool> AddRemoveModule(int CompanyID, int ModuleID, string ActionVal);
    }
}
