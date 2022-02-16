using CrisesControl.Core.CompanyAggregate.Handlers.GetCompanyList;
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

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCompanyList([FromQuery] GetCompanyListRequest request,
        CancellationToken cancellationToken)
    {
        var getCompanyListResponse = await _mediator.Send(request, cancellationToken);

        return Ok(getCompanyListResponse);
    }
}