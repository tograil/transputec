using CrisesControl.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Companies.Repositories;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetAllCompanies();
    Task<IEnumerable<Company>> GetAllCompanyList(int? status, string? companyProfile);
    Task<string> GetCompanyParameter(string key, int companyId, string @default = "", string customerId = "");
    Task<Company> GetCompanyByID(int companyId);
    Task<int> CreateCompany(Company company, CancellationToken token);

    Task<string> GetTimeZone(int companyId);

    Task<int> UpdateCompanyDRPlan(Company company);
    Task<int> UpdateCompanyLogo(Company company);
    Task<CompanyInfoReturn> GetCompany(CompanyRequestInfo company, CancellationToken token);
    Task<List<CommsMethod>> GetCommsMethod();
    Task<int> UpdateCompany(Company company);
    Task<bool> DuplicateCompany(string CompanyName, string Countrycode);
    Task<bool> DeleteCompanyApi(int CompanyId, string CustomerId);
    Task<dynamic> DeleteCompanyComplete(int CompanyId, int UserId, string GUID, string DeleteType);
    Task<AddressLink> GetCompanyAddress(int CompanyID);
    //Task<dynamic> GetSite(int SiteID, int CompanyID);
    Task<int> SaveSite(Site site);
    Task<Site> GetCompanySiteById(int SiteID, int CompanyID);
    Task<GetCompanyDataResponse> GetStarted(int CompanyID);
    Task<List<SocialIntegraion>> GetSocialIntegration(int CompanyID, string AccountType);
    Task<bool> SaveSocialIntegration(string AccountName, string AccountType, string AuthSecret, string AdnlKeyOne, string AuthToken, string AdnlKeyTwo, int CompanyId, string TimeZoneId, int userId);
    Task<Site> GetSite(int SiteID, int CompanyID);
    Task<List<Site>> GetSites(int CompanyID);

    Task<string> CheckFunds(int CompanyID, string UserRole);
    Task<ReplyChannel> GetReplyChannel(int CompanyID, int UserID);
    Task<CompanyCommunication> GetCompanyComms(int CompanyID, int UserID);
    Task<CompanyAccount> GetCompanyAccount(int CompanyID);
    Task<ReplyChannel> UpdateCompanyComms(int CompanyID, int[] MethodId, int[] BillingUsers, int CurrentAdmin, int CurrentUserID, string TimeZoneId, string Source = "PORTAL");
    Task<bool> CompanyDataReset(string[] ResetOptions, int CompanyID, string TimeZoneId);
    Task ResetGlobalConfig(int CompanyID, string TimeZoneId);
    Task ResetPings(int CompanyID);
    Task ResetActiveIncident(int CompanyID);
    Task<int> DeactivateCompany(Company company);
    Task<int> ReactivateCompany(Company company);
    Task<string> LookupWithKey(string Key, string Default = "");
}