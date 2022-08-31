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
        Task<dynamic> GetCompanyByKey(string activationKey, int outUserCompanyId);
        //Task<CompanyActivation> GetCompanyByKey(string ActivationKey, int OutUserCompanyId);
        Task<bool> OnTrialStatus(string companyProfile, bool currentTrial);
        Task<int> UpgradeByKey(Company company);
        Task<bool> UpgradePackage(UpdateCompanyPaymentProfileModel ip, int currentUserId, int outUserCompanyId, string timeZoneId);
        Task<int> UpdateCompanyTranscationType(int companyId, int currntUserId, string timeZoneId, int transactionTypeID, decimal transactionRate,
          int compnayTranscationTypeId = 0, string paymentPeriod = "MONTHLY", DateTimeOffset? nextRunDate = null, string paymentMethod = "INVOICE");
        Task<CompanyPaymentProfile> UpdateCompanyPaymentProfile(UpdateCompanyPaymentProfileModel ip, int currentUserId, int outUserCompanyId, string timeZoneId);
        Task<List<PackageItems>> GetPackageAddons(int outUserCompanyId, bool showAll = false);
        Task<CompanyPackage> GetCompanyPackageItems(int outUserCompanyId);
        Task UpdateCompanyPaymentProfileAsync(int companyId, int currntUserId, string paymentPeriod, decimal creditBalance, decimal creditLimit, decimal minimumBalance,
           decimal textUplift, decimal phoneUplift, decimal emailUplift, decimal pushUplift, decimal confUplift,
           decimal minimumTextRate, decimal minimumPhoneRate, decimal minimumEmailRate, decimal minimumPushRate, decimal minimumConfRate,
           string timeZoneId, DateTimeOffset contractAnniversary, string agreementNo, decimal maxTransactionLimit, DateTimeOffset contractStartDate,
           string cardType, string cardHolderName, string billingEmail, string billingAddress1, string billingAddress2, string city, string town, string postcode,
           string country);
        Task<bool> AddRemoveModule(int companyID, int moduleID, string actionVal);
    }
}
