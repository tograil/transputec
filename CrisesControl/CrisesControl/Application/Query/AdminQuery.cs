using AutoMapper;
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
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Administrator.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.SharedKernel.Utils;
using System.Data;

namespace CrisesControl.Api.Application.Query
{
    public class AdminQuery: IAdminQuery
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AdminQuery> _logger;
        private readonly IAdminRepository _adminRepository;
        private readonly ICurrentUser _currentUser;
        private readonly SendEmail SDE;
        public AdminQuery(IMapper mapper, ILogger<AdminQuery> logger, IAdminRepository administratorRepository, ICurrentUser currentUser)
        {
            this._logger=logger;
            this._mapper=mapper;
            this._adminRepository=administratorRepository;
            this._currentUser = currentUser;
        }

        public async Task<AddLibIncidentResponse> AddLibIncident(AddLibIncidentRequest request)
        {
            var IsLibIncidentExist = await _adminRepository.GetLibIncidentByName(request.Name);
            var response = new AddLibIncidentResponse();
            if (IsLibIncidentExist != null && IsLibIncidentExist.LibIncidentId == request.LibIncidentId)
            {
                IsLibIncidentExist.Name = request.Name;
                IsLibIncidentExist.Description = request.Description;
                IsLibIncidentExist.LibIncidentTypeId = request.LibIncidentTypeId;
                IsLibIncidentExist.LibIncodentIcon = request.LibIncidentIcon;
                IsLibIncidentExist.Severity = request.Severity;
                IsLibIncidentExist.Status = request.Status;
                IsLibIncidentExist.UpdatedBy = _currentUser.UserId;
                IsLibIncidentExist.UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);
                var LibIncidentId = await _adminRepository.UpdateLibIncident(IsLibIncidentExist);
                response.LibIncidentId = LibIncidentId;
               
            }
            else if (IsLibIncidentExist == null && request.LibIncidentId == 0)
            {
                LibIncident newLibIncident = new LibIncident()
                {
                    Name = request.Name,
                    Description = request.Description,
                    LibIncidentTypeId = request.LibIncidentTypeId,
                    LibIncodentIcon = request.LibIncidentIcon,
                    Severity = request.Severity,
                    Status = 1,
                    CreatedBy = _currentUser.UserId,
                    CreatedOn = DateTime.Now,
                    UpdatedBy = _currentUser.UserId,
                    UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone)
                };
               var newLibIncidentId= await _adminRepository.AddLibIncident(newLibIncident);
                response.LibIncidentId = newLibIncidentId;

            }
            return response;
        }

        public async Task<DeleteLibIncidentResponse> DeleteLibIncident(DeleteLibIncidentRequest request)
        {
            var DeleteLibIncident = await _adminRepository.GetLibIncidentById(request.LibIncidentId);
            var response = new DeleteLibIncidentResponse();
            if (DeleteLibIncident != null)
            {
                 var isDeleted= await  _adminRepository.DeleteLibIncident(DeleteLibIncident);
                if(isDeleted)
                response.Deleted = isDeleted;
                response.Message = "Data Deleted.";
            }
            else
            {
                response.Deleted = false;
                response.Message = "No record found.";
            }
            return response;
        }

        public async Task<DumpReportResponse> DumpReport(DumpReportRequest request)
        {
            string rFilePath = string.Empty;
            string rFileName = string.Empty;
            DataTable dataTable =await  _adminRepository.GetReportData(request.ReportID, request.ParamList, rFilePath, rFileName);
            var response = new DumpReportResponse();
            if (request.DownloadFile)
            {
                var data = await _adminRepository.ToCSVHighPerformance(dataTable, true, ",");
                using (StreamWriter SW = new StreamWriter(rFilePath, false))
                {
                    SW.Write(data);
                }
                response.result = rFileName;
            }
            else
            {
                DataSet dsData = new DataSet();
                dsData.Tables.Add(dataTable);
                response.result= dsData.Tables[0].ToString();
            }
            return response;
        }

        public async Task<GetAllLibIncidentResponse> GetAllLibIncident(GetAllLibIncidentRequest request)
        {
            var libIncidents = await _adminRepository.GetAllLibIncident();
            var response = _mapper.Map<List<LibIncident>>(libIncidents);
            var result = new GetAllLibIncidentResponse();
            result.data = response;
            result.Message = "Data loaded Successfully";
            return result;
        }

        public async Task<GetReportResponse> GetReport(GetReportRequest request)
        {
            try
            {
                var libIncidents = await _adminRepository.GetReportList(request.ReportId);
                var result = _mapper.Map<AdminReport>(libIncidents);
                var response = new GetReportResponse();
                if (result!=null)
                {
                    response.Data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }
                
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetLibIncidentResponse> GetLibIncident(GetLibIncidentRequest request)
        {
            try
            {
                var libIncidents = await _adminRepository.GetLibIncident(request.LibIncidentId);
                var result = _mapper.Map<AdminLibIncident>(libIncidents);
                var response = new GetLibIncidentResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UpdateLibIncidentResponse> UpdateLibIncident(UpdateLibIncidentRequest request)
        {
            try
            {
                var IsLibIncidentExist = await _adminRepository.GetLibIncidentByName(request.Name);
                var result = _mapper.Map<AdminReport>(IsLibIncidentExist);
                var response = new UpdateLibIncidentResponse();


                if (result != null && IsLibIncidentExist.LibIncidentId == request.LibIncidentId)
                {
                    IsLibIncidentExist.Name = request.Name;
                    IsLibIncidentExist.Description = request.Description;
                    IsLibIncidentExist.LibIncidentTypeId = request.LibIncidentTypeId;
                    IsLibIncidentExist.LibIncodentIcon = request.LibIncidentIcon;
                    IsLibIncidentExist.Severity = request.Severity;
                    IsLibIncidentExist.Status = request.Status;
                    IsLibIncidentExist.UpdatedBy = _currentUser.UserId;
                    IsLibIncidentExist.UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);
                    var LibIncidentID = await _adminRepository.UpdateLibIncident(IsLibIncidentExist);
                    response.LibIncidentId = LibIncidentID;
                    
                }
                else if (IsLibIncidentExist == null && request.LibIncidentId == 0)
                {
                    LibIncident newLibIncident = new LibIncident()
                    {
                        Name = request.Name,
                        Description = request.Description,
                        LibIncidentTypeId = request.LibIncidentTypeId,
                        LibIncodentIcon = request.LibIncidentIcon,
                        Severity = request.Severity,
                        Status = 1,
                        CreatedBy = _currentUser.UserId,
                        CreatedOn = DateTime.Now,
                        UpdatedBy = _currentUser.UserId,
                        UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone)
                    };
                 
                    var newLibIncidentID = await _adminRepository.AddLibIncident(newLibIncident);
                    response.LibIncidentId = newLibIncidentID;
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetAllLibIncidentTypeResponse> GetAllLibIncidentType(GetAllLibIncidentTypeRequest request)
        {
            try
            {
                var libIncidents = await _adminRepository.GetAllLibIncidentType();
                var result = _mapper.Map<List<LibIncidentType>>(libIncidents);
                var response = new GetAllLibIncidentTypeResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UpdateLibIncidentTypeResponse> UpdateLibIncidentType(UpdateLibIncidentTypeRequest request)
        {
            try
            {
                var LibIncidentTypeExist = await _adminRepository.GetLibIncidentTypeByName(request.Name);
                var response = new UpdateLibIncidentTypeResponse();
                if (LibIncidentTypeExist != null && request.LibIncidentTypeId == LibIncidentTypeExist.LibIncidentTypeId)
                {
                    LibIncidentTypeExist.Name = request.Name;
                    var LibTypeId = await _adminRepository.UpdateLibIncidentType(LibIncidentTypeExist);
                }
                else if (LibIncidentTypeExist == null && request.LibIncidentTypeId == 0)
                {
                    LibIncidentType newLibIncidentType = new LibIncidentType()
                    {
                        Name = request.Name
                    };
                    var LibTypeId = await _adminRepository.AddLibIncidentType(newLibIncidentType);
                    response.LibIncidentTypeId = LibTypeId;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetLibIncidentTypeResponse> GetLibIncidentType(GetLibIncidentTypeRequest request)
        {
            try
            {
                var libIncidentType = await _adminRepository.GetLibIncidentType(request.LibIncidentTypeId);
                var result = _mapper.Map<List<LibIncidentType>>(libIncidentType);
                var response = new GetLibIncidentTypeResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AddLibIncidentTypeResponse> AddLibIncidentType(AddLibIncidentTypeRequest request)
        {
            try
            {
                var LibIncidentTypeExist = await _adminRepository.GetLibIncidentTypeByName(request.Name);
                
                var response = new AddLibIncidentTypeResponse();

                if (LibIncidentTypeExist != null && request.LibIncidentTypeId == LibIncidentTypeExist.LibIncidentTypeId)
                {
                    LibIncidentTypeExist.Name = request.Name;
                    var libIncidentTypeId = await _adminRepository.UpdateLibIncidentType(LibIncidentTypeExist);
                    response.LibInclidentTypeId = libIncidentTypeId;
                    response.Message = "Data loaded Successfully";

                }
                else if (LibIncidentTypeExist == null && request.LibIncidentTypeId == 0)
                {
                    LibIncidentType newLibIncidentType = new LibIncidentType()
                    {
                        Name = request.Name
                    };
                    var newLibIncidentTypeId = await _adminRepository.AddLibIncidentType(newLibIncidentType);
                    response.LibInclidentTypeId = newLibIncidentTypeId;
                    response.Message = "Data loaded Successfully";
                }
            

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DeleteLibIncidentTypeResponse> DeleteLibIncidentType(DeleteLibIncidentTypeRequest request)
        {
            try
            {
                var DeleteLibIncident = await _adminRepository.GetLibIncidentTypeById(request.LibIncidentTypeId);
                var response = new DeleteLibIncidentTypeResponse();
                if (DeleteLibIncident != null)
                {
                    var isDeleted = await _adminRepository.DeleteLibIncidentType(DeleteLibIncident);
                    if (isDeleted) { 
                        response.Deleted = isDeleted;
                    response.Message = "Data Deleted.";
                    }
                }
                else
                {
                    response.Deleted = false;
                    response.Message = "No record found.";
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetCompanyPackageFeaturesResponse> GetCompanyPackageFeatures(GetCompanyPackageFeaturesRequest request)
        {
            try
            {
                var modules = await _adminRepository.GetCompanyModules(_currentUser.CompanyId);
                var feature = await _adminRepository.GetCompanyPackageFeatures(_currentUser.CompanyId);
                var result = _mapper.Map<List<CompanyPackageFeatureList>>(feature);
                var resultModules = _mapper.Map<List<CompanyPackageFeatureList>>(modules);
                var response = new GetCompanyPackageFeaturesResponse();
                if (result != null || resultModules!=null)
                {
                    response.Feature = result;
                    response.Modules = resultModules;
                }
                else
                {
                    response.Feature = result;
                    response.Modules = resultModules;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TestTemplateResponse> TestTemplate(TestTemplateRequest request)
        {
            try
            {
               
                string SMTPHost = await _adminRepository.LookupWithKey("SMTPHOST");
                string FromAddress = await _adminRepository.LookupWithKey("ALERT_EMAILFROM");

                var result = SDE.Email(request.ExtraEmailList.ToArray(), request.EmailContent, FromAddress, SMTPHost, request.EmailSubject);


                var response = new TestTemplateResponse();
                if (result)
                {
                    response.isEmailSent = result;
                    response.Message = "Email Sent";
                }
                else
                {
                    response.isEmailSent = result;
                    response.Message = "Email Not Sent";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SendCustomerNoticeResponse> SendCustomerNotice(SendCustomerNoticeRequest request)
        {
            try
            {

                var custNotice = await _adminRepository.SendCustomerNotice(request.EmailContent, request.EmailSubject, request.ExtraEmailList);
                var result = _mapper.Map<List<AdminUsersList>>(custNotice);
                var response = new SendCustomerNoticeResponse();
                if (result!=null)
                {
                    response.Data = result;
                    response.Message = "Customer has bee Notified.";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No Notice to Customer.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RestoreTemplateResponse> RestoreTemplate(RestoreTemplateRequest request)
        {
            try
            {

                
                var response = new RestoreTemplateResponse();
                if (request.Code.ToUpper() != "ALL")
                {
                    var custNotice = await _adminRepository.GetEmailTemplateByCode(request.Code, _currentUser.CompanyId);
                    var result = _mapper.Map<EmailTemplate>(custNotice);
                    if (result != null)
                    {
                        var templateId = await _adminRepository.RestoreTemplate(custNotice);
                        if (templateId) { 
                        response.template = templateId;                       
                        }
                    }
                    else
                    {
                        var tmpl = await _adminRepository.GetEmailTemplate(request.Code, "en", 0, 1, _currentUser.CompanyId, _currentUser.TimeZone);

                        response.Data = tmpl;
                       
                    }
                    return response;

                }

                return response;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetEmailFieldsResponse> GetEmailFields(GetEmailFieldsRequest request)
        {
            try
            {

                var custNotice = await _adminRepository.GetEmailFields(request.TemplateCode, request.FieldType);
                var result = _mapper.Map<List<EmailFieldLookup>>(custNotice);
                var response = new GetEmailFieldsResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetEmailTemplateResponse> GetEmailTemplate(GetEmailTemplateRequest request)
        {
            try
            {

                var custNotice = await _adminRepository.GetEmailTemplate(request.Code, request.Locale, request.TemplateID, request.Status, request.QCompanyID, _currentUser.TimeZone);
                var result = _mapper.Map<List<EmailTemplateList>>(custNotice);
                var response = new GetEmailTemplateResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SaveEmailTemplateResponse> SaveEmailTemplate(SaveEmailTemplateRequest request)
        {
            try
            {

                var custNotice = await _adminRepository.SaveEmailTemplate(request.TemplateID, request.Type, request.Code, request.Name, request.Description, request.HtmlData, request.EmailSubject, request.Locale, request.Status, _currentUser.UserId, request.QCompanyID);
                var result = _mapper.Map<int>(custNotice);
                var response = new SaveEmailTemplateResponse();
                if (result > 0)
                {
                    response.TemplateId = result;
                    response.Message = "Email Template Addedd";
                }
                else
                {
                    response.TemplateId = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AddTransactionResponse> AddTransaction(AddTransactionRequest request)
        {
            try
            {

                var trans = await _adminRepository.AddTransaction(request.IP, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
                var result = _mapper.Map<int>(trans);
                var response = new AddTransactionResponse();
                if (result > 0)
                {
                    response.TemplateId = result;
                    response.Message = "Transaction entry is successful";
                }
                else
                {
                    response.TemplateId = result;
                    response.Message = "Transaction was not successfull.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SubscribeModuleResponse> SubscribeModule(SubscribeModuleRequest request)
        {
            try
            {

                var trans = await _adminRepository.SubscribeModule(request.TransactionTypeId, request.PaymentPeriod, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
                var result = _mapper.Map<bool>(trans);
                var response = new SubscribeModuleResponse();
                if (result)
                {
                    response.Subscribe = result;
                    response.Message = "Transaction entry is successful";
                }
                else
                {
                    response.Subscribe = result;
                    response.Message = "Transaction was not successfull.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetTransactionTypeResponse> GetTransactionType(GetTransactionTypeRequest request)
        {
            try
            {

                var transType = await _adminRepository.GetTransactionType();
                var result = _mapper.Map<List<TransactionType>>(transType);
                var response = new GetTransactionTypeResponse();
                if (result !=null)
                {
                    response.Data = result;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetCompanyTransactionResponse> GetCompanyTransaction(GetCompanyTransactionRequest request)
        {
            try
            {

                var transType = await _adminRepository.GetCompanyTransactions(request.CompanyId, request.StartDate, request.EndDate);
                var result = _mapper.Map<List<TransactionList>>(transType);
                var response = new GetCompanyTransactionResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SaveLanguageItemResponse> SaveLanguageItem(SaveLanguageItemRequest request)
        {
            try
            {
                var response = new SaveLanguageItemResponse();
                if (request.LanguageItemID > 0)
                {
                    var item = await _adminRepository.GetLanguageById(request.LanguageItemID);
                    if (item != null)
                    {
                        item.ErrorCode = request.ErrorCode;
                        item.LangKey = request.LangKey;
                        item.LangValue = request.LangValue;
                        item.LastUpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);
                        item.Locale = request.Locale;
                        item.ObjectType = request.ObjectType;
                        item.Options = request.Options;
                        item.Title = request.Title;
                        var LanguageId=await  _adminRepository.UpdateLanguageItem(item);
                        response.LanguageId = LanguageId;
                    }
                   
                }
                else
                {
                    LanguageItem LI = new LanguageItem();
                    LI.ErrorCode = request.ErrorCode;
                    LI.LangKey = request.LangKey;
                    LI.LangValue = request.LangValue;
                    LI.LastUpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);
                    LI.Locale = request.Locale;
                    LI.ObjectType = request.ObjectType;
                    LI.Options = request.Options;
                    LI.Title = request.Title;
                    var LanguageId = await _adminRepository.SaveLanguageItem(LI);
                    response.LanguageId = LanguageId;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetAppLanguageResponse> GetAppLanguage(GetAppLanguageRequest request)
        {
            try
            {

                var appLanguages = await _adminRepository.GetAppLanguage(request.LangKey, request.Locale, request.LanguageItemID);
                var result = _mapper.Map<AppLanguages>(appLanguages);
                var response = new GetAppLanguageResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UpdatePackageItemResponse> UpdatePackageItem(UpdatePackageItemRequest request)
        {
            try
            {

            
                var response = new UpdatePackageItemResponse();
                if (request.PackageItemId > 0)
                {
                    var cp = await _adminRepository.GetCompanyPackageById(request.PackageItemId);
                    if (cp != null)
                    {
                        cp.ItemValue = request.ItemValue;
                        cp.UpdatedBy = _currentUser.UserId;
                        cp.UpdatedOn = DateTime.Now.GetDateTimeOffset();
                        var PackageItemId = await _adminRepository.UpdateCompanyPackageItem(cp);
                        response.PackageId= PackageItemId;
                        response.Message = "Package Added";
                    }
                    else
                    {
                        response.PackageId = 0;
                        response.Message = "No record found.";
                    }
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetCompanyPackageItemsResponse> GetCompanyPackageItems(GetCompanyPackageItemsRequest request)
        {
            try
            {

                var appLanguages = await _adminRepository.GetCompanyPackageItems(_currentUser.CompanyId, request.PackageItemId);
                var result = _mapper.Map<CompanyPackageItems>(appLanguages);
                var response = new GetCompanyPackageItemsResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
