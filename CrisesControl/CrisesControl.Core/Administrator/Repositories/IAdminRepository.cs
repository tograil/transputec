using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CrisesControl.Core.Administrator.Repositories
{
    public interface IAdminRepository
    {
        Task<List<LibIncident>> GetAllLibIncident();
        Task<DataTable> GetReportData(int ReportID, List<ReportParam> sqlParams, string rFilePath, string rFileName);
        Task<string> ToCSVHighPerformance(DataTable dataTable, bool includeHeaderAsFirstRow = true, string separator = ",");
        Task<AdminResult> GetReportList(int ReportID);
        Task<bool> DeleteLibIncidentType(LibIncidentType libIncidentType);
        Task<bool> DeleteLibIncident(LibIncident libIncident);
        Task<int> AddLibIncident(LibIncident libIncident);
        Task<int> UpdateLibIncident(LibIncident libIncident);
        Task<int> AddLibIncidentType(LibIncidentType libIncidentType);
        Task<int> UpdateLibIncidentType(LibIncidentType libIncidentType);
        Task<LibIncident> GetLibIncidentByName(string Name);
        Task<LibIncidentType> GetLibIncidentTypeByName(string Name);
        Task<AdminLibIncident> GetLibIncident(int LibIncidentId);
        Task<LibIncident> GetLibIncidentById(int LibIncidentId);
        Task<List<LibIncidentType>> GetAllLibIncidentType();
        Task<LibIncidentType> GetLibIncidentType(int LibIncidentTypeId);
        Task<LibIncidentType> GetLibIncidentTypeById(int LibIncidentTypeId);
        Task<List<CompanyPackageFeatureList>> GetCompanyPackageFeatures(int OutUserCompanyId);
        Task<List<CompanyPackageFeatureList>> GetCompanyModules(int OutUserCompanyId);
        Task<EmailTemplate> GetEmailTemplateByCode(string TemplateCode, int CompanyID);
        Task<List<EmailTemplateList>> GetEmailTemplate(string Code, string Locale, int TemplateID, int Status, int CompanyID, string TimeZoneId);
        Task<EmailTemplate> GetEmailTemplateById(int TemplateId, int CompanyID);
        Task<List<EmailFieldLookup>> GetEmailFields(string TemplateCode, int FieldType = 1);
        Task<int> CreateEmailTemplate(string Type, string Code, string Name, string Description, string HtmlData, string EmailSubject, string Locale,
            int Status, int CurrentUserID, int CompanyID, string TimeZoneId);
        Task<int> SaveEmailTemplate(int TemplateID, string Type, string Code, string Name, string Description, string HtmlData, string EmailSubject,
            string Locale, int Status, int CurrentUserID, int CompanyID = 0, string TimeZoneId = "GMT Standard Time");
        Task<string> LookupWithKey(string Key, string Default = "");
        Task<List<AdminUsersList>> SendCustomerNotice(string EmailContent, string EmailSubject, List<string> ExtraEmailList);
        Task<bool> RestoreTemplate(EmailTemplate ctemplate);
        Task<int> AddTransaction(UpdateTransactionDetailsModel IP, int CurrentUserId, int CompanyId, string TimeZoneId);
        Task<List<TransactionType>> GetTransactionType();
        Task<int> UpdateTransactionDetails(int TransactionHeaderId, int CompanyId, int TransactionTypeId, decimal TransactionRate, decimal MinimumPrice,
           int Quantity, decimal Cost, decimal LineValue, decimal LineVAT, decimal Total, int MessageId, DateTimeOffset TransactionDate, int currntUserId = 0,
           string TransactionReference = "", string TimeZoneId = "GMT Standard Time", int TransactionStatus = 1, int TransactionDetailsId = 0, string TrType = "DR");
        Task CreateCompanyPackageFeature(int SecurityObjectID, int CompanyID, int Status);
        Task<bool> SubscribeModule(int TransactionTypeId, string PaymentPeriod, int CurrentUserId, int CompanyId, string TimeZoneId);
        Task UpdateCompanyParameter(string Name, string Value, int CompanyId, int CurrentUserId, string TimeZoneId, int Status = 4);
        Task<TransactionList> GetCompanyTransactions(int CompanyId, DateTimeOffset StartDate, DateTimeOffset EndDate);
        Task<List<UnpaidTransaction>> GetUnpaidTransactions(int TransactionId, DateTimeOffset StartDate, DateTimeOffset EndDate);
        Task UpdateCompanyPackageItem(string ItemCode, string ItemValue, int Status, int CompanyID, int CurrentUserId, string TimeZoneId);
        Task<string> GetPackageItem(string ItemCode, int CompanyId);
        Task<int> AddCompanyParameter(string Name, string Value, int CompanyId, int CurrentUserId, string TimeZoneId);
        Task GetSetCompanyComms(int CompanyID);
        Task _set_comms_status(int CompanyId, List<string> methods, bool status);
        Task<LanguageItem> GetLanguageById(int lanuageId);
        Task<int> SaveLanguageItem(LanguageItem lanuage);
        Task<int> UpdateLanguageItem(LanguageItem lanuage);
        Task<AppLanguages> GetAppLanguage(string LangKey, string Locale, int LanguageItemID, string ObjectType = "APP");
        Task<CompanyPackageItem> GetCompanyPackageById(int PackageItemId);
        Task<int> UpdateCompanyPackageItem(CompanyPackageItem packageItem);
        Task<CompanyPackageItems> GetCompanyPackageItems(int CompanyID, int PackageItemId);
        Task<SysParameter> GetSysParameters(int SysParametersId);
        Task<List<SysParameter>> GetAllSysParameters();


    }
}
