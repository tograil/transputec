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

namespace CrisesControl.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class CompanyController : Controller
{
    private readonly IMediator _mediator;

    public CompanyController(IMediator mediator)
    {
        _mediator = mediator;
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
}