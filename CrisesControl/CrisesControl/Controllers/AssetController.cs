using CrisesControl.Api.Application.Commands.Assets.CreateAsset;
using CrisesControl.Api.Application.Commands.Assets.DeleteAsset;
using CrisesControl.Api.Application.Commands.Assets.DeleteAssetLink;
using CrisesControl.Api.Application.Commands.Assets.GetAllAssets;
using CrisesControl.Api.Application.Commands.Assets.GetAsset;
using CrisesControl.Api.Application.Commands.Assets.GetAssetLink;
using CrisesControl.Api.Application.Commands.Assets.GetAssets;
using CrisesControl.Api.Application.Commands.Assets.UpdateAssets;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssetModel = CrisesControl.Core.Assets.Assets;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AssetController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IAssetQuery _assetQuery;


        public AssetController(IMediator mediator, IAssetQuery assetQuery) {
            _mediator = mediator;
            _assetQuery = assetQuery;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllAssets([FromQuery] GetAllAssetsRequest request, CancellationToken cancellationToken)
        {
            var result = await _assetQuery.GetAllAssets(request);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAssets([FromRoute] GetAssetsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("[action]/{AssetId}")]
        public async Task<IActionResult> GetAssetLink([FromRoute] GetAssetLinkRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("{AssetId:int}")]
        public async Task<IActionResult> GetAsset([FromRoute] GetAssetRequest request, CancellationToken cancellationToken)
        {
            var result = await _assetQuery.GetAsset(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsset([FromBody] CreateAssetRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        [Route("{AssetId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateAsset([FromBody] UpdateAssetsRequest assetModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(assetModel, cancellationToken);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{AssetId:int}")]
        public async Task<IActionResult> DeleteAsset([FromRoute] DeleteAssetRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpDelete]
        [Route("[action]/{AssetId}")]
        public async Task<IActionResult> DeleteAssetLink([FromRoute] DeleteAssetLinkRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
