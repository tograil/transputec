using CrisesControl.Api.Application.Commands.Administrator.AddApiUrls;
using CrisesControl.Api.Application.Commands.Administrator.AddLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.AddLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.AddSysParameters;
using CrisesControl.Api.Application.Commands.Administrator.AddTransaction;
using CrisesControl.Api.Application.Commands.Administrator.ApiUrls;
using CrisesControl.Api.Application.Commands.Administrator.ApiUrlsById;
using CrisesControl.Api.Application.Commands.Administrator.CreateActivationKey;
using CrisesControl.Api.Application.Commands.Administrator.DeleteApiUrl;
using CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.DumpReport;
using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.GetAllSysParameters;
using CrisesControl.Api.Application.Commands.Administrator.GetAppLanguage;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyDetails;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyGlobalReport;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyModules;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageFeatures;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageItems;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyTransaction;
using CrisesControl.Api.Application.Commands.Administrator.GetEmailFields;
using CrisesControl.Api.Application.Commands.Administrator.GetEmailTemplate;
using CrisesControl.Api.Application.Commands.Administrator.GetLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.GetLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.GetMonthlyTransaction;
using CrisesControl.Api.Application.Commands.Administrator.GetReport;
using CrisesControl.Api.Application.Commands.Administrator.GetSysParameters;
using CrisesControl.Api.Application.Commands.Administrator.GetTag;
using CrisesControl.Api.Application.Commands.Administrator.GetTagCategory;
using CrisesControl.Api.Application.Commands.Administrator.GetTransactionType;
using CrisesControl.Api.Application.Commands.Administrator.GetUnpaidTransactions;
using CrisesControl.Api.Application.Commands.Administrator.RebuildJobs;
using CrisesControl.Api.Application.Commands.Administrator.RestoreTemplate;
using CrisesControl.Api.Application.Commands.Administrator.SaveContractOffer;
using CrisesControl.Api.Application.Commands.Administrator.SaveEmailTemplate;
using CrisesControl.Api.Application.Commands.Administrator.SaveLanguageItem;
using CrisesControl.Api.Application.Commands.Administrator.SaveTag;
using CrisesControl.Api.Application.Commands.Administrator.SaveTagCategory;
using CrisesControl.Api.Application.Commands.Administrator.SendCustomerNotice;
using CrisesControl.Api.Application.Commands.Administrator.SubscribeModule;
using CrisesControl.Api.Application.Commands.Administrator.TestTemplate;
using CrisesControl.Api.Application.Commands.Administrator.UpdateApiUrls;
using CrisesControl.Api.Application.Commands.Administrator.UpdateCustomer;
using CrisesControl.Api.Application.Commands.Administrator.UpdateLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.UpdateLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.UpdatePackageItem;
using CrisesControl.Api.Application.Commands.Administrator.UpdateSysParameters;
using CrisesControl.Api.Application.Query;
using CrisesControl.Api.Maintenance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CrisesControl.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class AdminController : Controller
{
    private readonly IMediator _mediator;
    private readonly ICompanyQuery _companyQuery;

    public AdminController(IMediator mediator,
        ICompanyQuery companyQuery)
    {
        _mediator = mediator;
        _companyQuery = companyQuery;
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> GetAllCompanyList([FromQuery] int? status,
        [FromQuery] string? companyProfile,
        CancellationToken cancellationToken)
    {
        var companies = await _companyQuery.GetCompanyList(status, companyProfile);

        return Ok(companies);
    }
    [HttpGet]
    [Route("GetAllLibIncident")]
    public async Task<IActionResult> GetAllLibIncident([FromRoute] GetAllLibIncidentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("DumpReport/{ReportID}/{DownloadFile}")]
    public async Task<IActionResult> DumpReport([FromRoute] DumpReportRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetReport/{ReportId}")]
    public async Task<IActionResult> GetReport([FromRoute] GetReportRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetLibIncident/{LibIncidentId}")]
    public async Task<IActionResult> GetLibIncident([FromRoute] GetLibIncidentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("AddLibIncident")]
    public async Task<IActionResult> AddLibIncident([FromBody] AddLibIncidentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpPut]
    [Route("UpdateLibIncident")]
    public async Task<IActionResult> UpdateLibIncident([FromBody] UpdateLibIncidentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpDelete]
    [Route("DeleteLibIncident/{LibIncidentId}")]
    public async Task<IActionResult> DeleteLibIncident([FromRoute] DeleteLibIncidentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetAllLibIncidentType")]
    public async Task<IActionResult> GetAllLibIncidentType([FromRoute] GetAllLibIncidentTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetLibIncidentType/{LibIncidentTypeId}")]
    public async Task<IActionResult> GetLibIncidentType([FromRoute] GetLibIncidentTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetCompanyPackageFeatures")]
    public async Task<IActionResult> GetCompanyPackageFeatures([FromRoute] GetCompanyPackageFeaturesRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpPost]
    [Route("AddLibIncidentType")]
    public async Task<IActionResult> AddLibIncidentType([FromBody] AddLibIncidentTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpPut]
    [Route("UpdateLibIncidentType")]
    public async Task<IActionResult> UpdateLibIncidentType([FromBody] UpdateLibIncidentTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpDelete]
    [Route("DeleteLibIncidentType/{LibIncidentTypeId}")]
    public async Task<IActionResult> DeleteLibIncidentType([FromRoute] DeleteLibIncidentTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("GetEmailTemplate")]
    public async Task<IActionResult> GetEmailTemplate([FromRoute] GetEmailTemplateRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpPost]
    [Route("SaveEmailTemplate")]
    public async Task<IActionResult> SaveEmailTemplate([FromBody] SaveEmailTemplateRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetEmailFields/{TemplateCode}/{FieldType}")]
    public async Task<IActionResult> GetEmailFields([FromRoute] GetEmailFieldsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpDelete]
    [Route("RestoreTemplate/{Code}")]
    public async Task<IActionResult> RestoreTemplate([FromRoute] RestoreTemplateRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("SendCustomerNotice/{EmailContent}/{EmailSubject}/{ExtraEmailList}")]
    public async Task<IActionResult> SendCustomerNotice([FromRoute] SendCustomerNoticeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpDelete]
    [Route("TestTemplate/{EmailContent}/{EmailSubject}/{ExtraEmailList}")]
    public async Task<IActionResult> TestTemplate([FromRoute] TestTemplateRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpPost]
    [Route("AddTransaction")]
    public async Task<IActionResult> AddTransaction([FromBody] AddTransactionRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpPut]
    [Route("SubscribeModule")]
    public async Task<IActionResult> SubscribeModule([FromRoute] SubscribeModuleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetTransactionType")]
    public async Task<IActionResult> GetTransactionType([FromRoute] GetTransactionTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetCompanyTransactions/{CompanyId}/{StartDate}/{EndDate}")]
    public async Task<IActionResult> GetCompanyTransactions([FromRoute] GetCompanyTransactionRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetCompanyPackageItems/{PackageItemId}")]
    public async Task<IActionResult> GetCompanyPackageItems([FromRoute] GetCompanyPackageItemsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetAppLanguage/{LanguageItemID}/{LangKey}/{Locale}/{ObjectType}")]
    public async Task<IActionResult> GetAppLanguage([FromRoute] GetAppLanguageRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpPost]
    [Route("SaveLanguageItem")]
    public async Task<IActionResult> SaveLanguageItem([FromBody] SaveLanguageItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpPut]
    [Route("UpdatePackageItem/{PackageItemId}/{ItemValue}")]
    public async Task<IActionResult> UpdatePackageItem([FromRoute] UpdatePackageItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Get the System Parameters
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetSysParameters")]
    public async Task<IActionResult> GetSysParameters([FromRoute] GetSysParametersRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Get the systems parameters
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetAllSysParameters")]
    public async Task<IActionResult> GetAllSysParameters([FromRoute] GetAllSysParametersRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Add systems parameters
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("AddSysParameters")]
    public async Task<IActionResult> AddSysParameters([FromBody] AddSysParametersRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Update systems parameters
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("UpdateSysParameters")]
    public async Task<IActionResult> UpdateSysParameters([FromBody] UpdateSysParametersRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Get Unpaid Transactions
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("GetUnpaidTransactions")]
    public async Task<IActionResult> GetUnpaidTransactions([FromRoute] GetUnpaidTransactionsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Create activation Key
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("CreateActivationKey")]
    public async Task<IActionResult> CreateActivationKey([FromRoute] CreateActivationKeyRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Get tag Category
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetTagCategory/{TagCategoryID}")]
    public async Task<IActionResult> GetTagCategory([FromRoute] GetTagCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Save Category Tag
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("SaveTagCategory")]
    public async Task<IActionResult> SaveTagCategory([FromBody] SaveTagCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Get Tag
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetTag/{TagId}")]
    public async Task<IActionResult> GetTag([FromRoute] GetTagRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Save a Tag
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("SaveTag")]
    public async Task<IActionResult> SaveTag([FromBody] SaveTagRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Get Monthly Transactions
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("GetMonthlyTransaction")]
    public async Task<IActionResult> GetMonthlyTransaction([FromRoute] GetMonthlyTransactionRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Save a contract offer
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("SaveContractOffer")]
    public async Task<IActionResult> SaveContractOffer([FromBody] SaveContractOfferRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Get a Company Modules
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("GetCompanyModules")]
    public async Task<IActionResult> GetCompanyModules([FromBody] GetCompanyModulesRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Rebuild Jobs
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("RebuildJobs/{Company}/{JobType}")]
    public async Task<IActionResult> RebuildJobs([FromBody] RebuildJobsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Add Api URl
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("ApiUrls/Add")]
    public async Task<IActionResult> ApiUrls([FromBody] AddApiUrlsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Get Api ulrs
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("ApiUrls")]
    public async Task<IActionResult> ApiUrls([FromRoute] ApiUrlsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Get ApiUrls 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("ApiUrls/{ApiID}")]
    public async Task<IActionResult> GetApiUrls([FromRoute] ApiUrlsByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Update API Urls
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("ApiUrls/{id:int}/Update")]
    public async Task<IActionResult> ApiUrls([FromRoute] int id, [FromBody] UpdateApiUrlsRequest request, CancellationToken cancellationToken)
    {
        if (request.Api.ApiId != id) { 
            ModelState.AddModelError("ApiId", "Conflicting resource ID and model ID.");
        }
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Delete API Urls
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("ApiUrls/{ApiID:int}/Delete")]
    public async Task<IActionResult> ApiUrls([FromRoute] DeleteApiUrlRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Administrator Get Company Details 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetCompanyDetails/{ApiID:int}/Delete")]
    public async Task<IActionResult> GetCompanyDetails([FromRoute] GetCompanyDetailsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Administrator Update  Customer by Id 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("UpdateCustomer/{QCustomerId:int}")]
    public async Task<IActionResult> UpdateCustomer([FromRoute] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Get Company Global Report
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetCompanyGlobalReport")]
    public async Task<IActionResult> GetCompanyGlobalReport([FromRoute] GetCompanyGlobalReportRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}