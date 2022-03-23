using CrisesControl.Api.Application.Commands.MediaAssets.CreateAsset;
using CrisesControl.Api.Application.Commands.MediaAssets.GetAsset;
using CrisesControl.Api.Application.Commands.MediaAssets.GetAssets;
using CrisesControl.Api.Application.Commands.MediaAssets.UpdateAssets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssetModel = CrisesControl.Core.AssetAggregate.Asset;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("/api/[controller]")]
    public class AssetController : Controller
    {
        private readonly IMediator _mediator;

        public AssetController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] GetAssetsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("detail")]
        public async Task<IActionResult> GetAsset([FromQuery] GetAssetRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsset([FromBody] CreateAssetRequest assetModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(assetModel, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsset([FromBody] UpdateAssetsRequest assetModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(assetModel, cancellationToken);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDepartment([FromBody] AssetModel assetModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(assetModel, cancellationToken);
            return Ok(result);
        }
    }
}
