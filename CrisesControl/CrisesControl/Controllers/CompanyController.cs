using CrisesControl.Api.Application.Commands.Companies.UpdateCompanyDRPlan;
using CrisesControl.Api.Application.Commands.Companies.UpdateCompanyLogo;
using CrisesControl.Api.Application.Query;
using CrisesControl.Api.Application.Commands.Companies.GetCommsMethod;
using CrisesControl.Api.Application.Commands.Companies.GetCompany;
using CrisesControl.Api.Application.Commands.Companies.UpdateCompany;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrisesControl.Api.Application.Commands.Companies.CheckCompany;
using CrisesControl.Api.Application.Commands.Companies.DeleteCompany;
using CrisesControl.Api.Application.Commands.Companies.ViewCompany;
using CrisesControl.Api.Application.Commands.Companies.GetSite;
using CrisesControl.Api.Application.Commands.Companies.SaveSite;
using CrisesControl.Api.Application.Commands.GetStarted;
using CrisesControl.Api.Application.Commands.Companies.GetSocialIntegration;
using CrisesControl.Api.Application.Commands.Companies.SaveSocialIntegration;
using CrisesControl.Api.Application.Commands.Companies.GetCompanyComms;
using CrisesControl.Api.Application.Commands.Companies.GetCompanyAccount;
using CrisesControl.Api.Application.Commands.Companies.UpdateCompanyComms;
using CrisesControl.Api.Application.Commands.Companies.CompanyDataReset;
using CrisesControl.Api.Application.Commands.Companies.DeactivateCompany;
using CrisesControl.Api.Application.Commands.Companies.ReactivateCompany;
using CrisesControl.Api.Application.Commands.Companies.DeleteCompanyComplete;
using CrisesControl.Api.Application.Commands.Companies.GetCompanyObject;
using CrisesControl.Api.Application.Commands.Companies.GetGroupUsers;
using CrisesControl.Api.Application.Commands.Companies.GetScimProfile;
using CrisesControl.Api.Application.Commands.Companies.SaveScimProfile;

namespace CrisesControl.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class CompanyController : Controller
{
    private readonly IMediator _mediator;
    private readonly ICompanyQuery _companyQuery;

    public CompanyController(IMediator mediator, ICompanyQuery companyQuery)
    {
        _mediator = mediator;
        _companyQuery = companyQuery;
    }

    [HttpGet]
    [Route("{CompanyId:int}")]
    public async Task<IActionResult> GetCompany([FromRoute] GetCompanyRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("GetCommsMethod")]
    public async Task<IActionResult> GetCommsMethod([FromRoute] GetCommsMethodRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("GetSocialIntergration")]
    public async Task<IActionResult> GetSocialIntergration()
    {
        try
        {
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut]
    public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpPut]
    [Route("UpdateCompanyDRP")]
    public async Task<IActionResult> UpdateCompanyDRP([FromBody] UpdateCompanyDRPlanRequest request,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    [HttpPut]
    [Route("UpdateCompanyLogo")]
    public async Task<IActionResult> UpdateCompanyLogo([FromBody] UpdateCompanyLogoRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    [HttpGet]
    [Route("CheckCompany/{CompanyName}/{CountryCode}")]
    public async Task<IActionResult> CheckCompany([FromRoute] CheckCompanyRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    [HttpDelete]
    [Route("DeleteCompany/{CompanyId}/{UserId}/{GUID}/{DeleteType}")]
    public async Task<IActionResult> DeleteCompany([FromRoute] DeleteCompanyRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    [HttpGet]
    [Route("ViewCompany/{CompanyId}")]
    public async Task<IActionResult> ViewCompany([FromRoute] ViewCompanyRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    [HttpGet]
    [Route("GetSite/{SiteId}/{CompanyId}")]
    public async Task<IActionResult> GetSite([FromRoute] GetSiteRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    [HttpPost]
    [Route("SaveSite")]
    public async Task<IActionResult> SaveSite([FromBody] SaveSiteRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    [HttpGet]
    [Route("GetStarted/{CompanyId}")]
    public async Task<IActionResult> GetStarted([FromRoute] GetStartedRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    [HttpGet]
    [Route("GetSocialIntegration/{CompanyID}/{AccountType}")]
    public async Task<IActionResult> GetSocialIntegration([FromRoute] GetSocialIntegrationRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    [HttpPost]
    [Route("SaveSocialIntegration")]
    public async Task<IActionResult> SaveSocialIntegrationRequest([FromBody] SaveSocialIntegrationRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    [HttpGet]
    [Route("GetCompanyComms/{CompanyId:int}")]
    public async Task<IActionResult> GetCompanyComms([FromRoute] GetCompanyCommsRequest request, CancellationToken cancellationToken)
    {
        var result = await _companyQuery.GetCompanyComms(request);

        return Ok(result);
    }
    [HttpGet]
    [Route("GetCompanyAccount")]
    public async Task<IActionResult> GetCompanyAccount([FromRoute] GetCompanyAccountRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    [HttpPut]
    [Route("UpdateCompanyComms")]
    public async Task<IActionResult> UpdateCompanyComms([FromBody] UpdateCompanyCommsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    [HttpGet]
    [Route("CompanyDataReset/{ResetOptions}")]
    public async Task<IActionResult> CompanyDataReset([FromRoute] CompanyDataResetRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    [HttpPut]
    [Route("DeactivateCompany/{TargetCompanyID}")]
    public async Task<IActionResult> DeactivateCompany([FromRoute] DeactivateCompanyRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    [HttpPut]
    [Route("ReactivateCompany/{ActivateReactivateCompanyId}")]
    public async Task<IActionResult> ReactivateCompany([FromRoute] ReactivateCompanyRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    /// <summary>
    /// Delete Company Complete.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("[action]/{CompanyId}/{UserId}/{GUID}/{DeleteType}")]
    public async Task<IActionResult> DeleteCompanyComplete([FromRoute] DeleteCompanyCompleteRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Get Company Object.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]/{ObjectName}")]
    public async Task<IActionResult> GetCompanyObject([FromRoute] GetCompanyObjectRequest request, CancellationToken cancellationToken)
    {
        var result = await _companyQuery.GetCompanyObject(request);
        return Ok(result);
    }
    /// <summary>
    /// Get Group Users.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]/{GroupId}/{ObjectMappingId}")]
    public async Task<IActionResult> GetGroupUsers([FromRoute] GetGroupUsersRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Get Scim Profile.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> GetScimProfile([FromRoute] GetScimProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Save Scim Profile.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> SaveScimProfile([FromBody] SaveScimProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}