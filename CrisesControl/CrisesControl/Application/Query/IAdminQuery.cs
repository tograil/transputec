using CrisesControl.Api.Application.Commands.Administrator.AddLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.AddLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.AddTransaction;
using CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.DumpReport;
using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.GetAppLanguage;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageFeatures;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageItems;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyTransaction;
using CrisesControl.Api.Application.Commands.Administrator.GetEmailFields;
using CrisesControl.Api.Application.Commands.Administrator.GetEmailTemplate;
using CrisesControl.Api.Application.Commands.Administrator.GetLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.GetLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.GetReport;
using CrisesControl.Api.Application.Commands.Administrator.GetTransactionType;
using CrisesControl.Api.Application.Commands.Administrator.RestoreTemplate;
using CrisesControl.Api.Application.Commands.Administrator.SaveEmailTemplate;
using CrisesControl.Api.Application.Commands.Administrator.SaveLanguageItem;
using CrisesControl.Api.Application.Commands.Administrator.SendCustomerNotice;
using CrisesControl.Api.Application.Commands.Administrator.SubscribeModule;
using CrisesControl.Api.Application.Commands.Administrator.TestTemplate;
using CrisesControl.Api.Application.Commands.Administrator.UpdateLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.UpdateLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.UpdatePackageItem;

namespace CrisesControl.Api.Application.Query
{
    public interface IAdminQuery
    {
        Task<GetAllLibIncidentResponse> GetAllLibIncident(GetAllLibIncidentRequest request);
        Task<DumpReportResponse> DumpReport(DumpReportRequest request);
        Task<GetReportResponse> GetReport(GetReportRequest request);
        Task<AddLibIncidentResponse> AddLibIncident(AddLibIncidentRequest request);
        Task<UpdateLibIncidentResponse> UpdateLibIncident(UpdateLibIncidentRequest request);
        Task<DeleteLibIncidentResponse> DeleteLibIncident(DeleteLibIncidentRequest request);
        Task<GetLibIncidentResponse> GetLibIncident(GetLibIncidentRequest request);
        Task<GetAllLibIncidentTypeResponse> GetAllLibIncidentType(GetAllLibIncidentTypeRequest request);
        Task<UpdateLibIncidentTypeResponse> UpdateLibIncidentType(UpdateLibIncidentTypeRequest request);
        Task<GetLibIncidentTypeResponse> GetLibIncidentType(GetLibIncidentTypeRequest request);
        Task<AddLibIncidentTypeResponse> AddLibIncidentType(AddLibIncidentTypeRequest request);
        Task<DeleteLibIncidentTypeResponse> DeleteLibIncidentType(DeleteLibIncidentTypeRequest request);
        Task<GetCompanyPackageFeaturesResponse> GetCompanyPackageFeatures(GetCompanyPackageFeaturesRequest request);
        Task<TestTemplateResponse> TestTemplate(TestTemplateRequest request);
        Task<SendCustomerNoticeResponse> SendCustomerNotice(SendCustomerNoticeRequest request);
        Task<RestoreTemplateResponse> RestoreTemplate(RestoreTemplateRequest request);
        Task<GetEmailFieldsResponse> GetEmailFields(GetEmailFieldsRequest request);
        Task<GetEmailTemplateResponse> GetEmailTemplate(GetEmailTemplateRequest request);
        Task<SaveEmailTemplateResponse> SaveEmailTemplate(SaveEmailTemplateRequest request);
        Task<AddTransactionResponse> AddTransaction(AddTransactionRequest request);
        Task<SubscribeModuleResponse> SubscribeModule(SubscribeModuleRequest request);
        Task<GetTransactionTypeResponse> GetTransactionType(GetTransactionTypeRequest request);
        Task<GetCompanyTransactionResponse> GetCompanyTransaction(GetCompanyTransactionRequest request);
        Task<SaveLanguageItemResponse> SaveLanguageItem(SaveLanguageItemRequest request);
        Task<GetAppLanguageResponse> GetAppLanguage(GetAppLanguageRequest request);
        Task<UpdatePackageItemResponse> UpdatePackageItem(UpdatePackageItemRequest request);
        Task<GetCompanyPackageItemsResponse> GetCompanyPackageItems(GetCompanyPackageItemsRequest request);
    }
}
