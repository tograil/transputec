using CrisesControl.Api.Application.Commands.Administrator.AddLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.AddLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.AddTransaction;
using CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.DumpReport;
using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.GetAllSysParameters;
using CrisesControl.Api.Application.Commands.Administrator.GetAppLanguage;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageFeatures;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageItems;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyTransaction;
using CrisesControl.Api.Application.Commands.Administrator.GetEmailFields;
using CrisesControl.Api.Application.Commands.Administrator.GetEmailTemplate;
using CrisesControl.Api.Application.Commands.Administrator.GetLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.GetLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.GetReport;
using CrisesControl.Api.Application.Commands.Administrator.GetSysParameters;
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
using CrisesControl.Api.Application.Query;
using CrisesControl.Api.Maintenance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}