using CrisesControl.Api.Application.Commands.Locations.CreateLocation;
using CrisesControl.Api.Application.Commands.Locations.DeleteLocation;
using CrisesControl.Api.Application.Commands.Locations.GetLocation;
using CrisesControl.Api.Application.Commands.Locations.GetLocations;
using CrisesControl.Api.Application.Commands.Locations.SegregationLinks;
using CrisesControl.Api.Application.Commands.Locations.UpdateLocation;
using CrisesControl.Api.Application.Commands.Locations.UpdateSegregationLink;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers;
[ApiController]
[Route("/api/[controller]")]
public class LocationController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILocationQuery _locationQuery;

    public LocationController(IMediator mediator, ILocationQuery locationQuery)
    {
        _mediator = mediator;
        _locationQuery = locationQuery;
    }

    [HttpGet]
    [Route("{CompanyId:int}")]
    public async Task<IActionResult> Index([FromRoute] GetLocationsRequest request, CancellationToken cancellationToken)
    {
        var result = await _locationQuery.GetLocations(request, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Route("{CompanyId:int}/{LocationId:int}")]
    public async Task<IActionResult> GetLocation([FromRoute] GetLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await _locationQuery.GetLocation(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> CreateLocation([FromBody] CreateLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    /// <summary>
    /// Delete Location from the database
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("[action]")]
    public async Task<IActionResult> DeleteLocation([FromRoute] DeleteLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    /// <summary>
    /// Get Segregation Links
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("[action]/{TargetID}/{MemberShipType}/{LinkType}")]
    public async Task<IActionResult> SegregationLinks([FromRoute] SegregationLinksRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    /// <summary>
    /// Update the Segregation Links
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    //[HttpPut]
    //[Route("[action]")]
    //public async Task<IActionResult> UpdateSegregationLink([FromBody] UpdateSegregationLinkRequest request, CancellationToken cancellationToken)
    //{
    //    var result = await _mediator.Send(request, cancellationToken);

    //    return Ok(result);
    //}
}