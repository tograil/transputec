using CrisesControl.Api.Application.Commands.Locations.CreateLocation;
using CrisesControl.Api.Application.Commands.Locations.GetLocation;
using CrisesControl.Api.Application.Commands.Locations.GetLocations;
using CrisesControl.Api.Application.Commands.Locations.UpdateLocation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers;
[ApiController]
[Route("/api/[controller]")]
[AllowAnonymous]
public class LocationController : Controller
{
    private readonly IMediator _mediator;

    public LocationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] GetLocationsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetLocation([FromQuery] GetLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] CreateLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
}