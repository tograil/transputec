using CrisesControl.Api.Application.Commands.Assets.CreateAsset;
using CrisesControl.Api.Application.Commands.Assets.GetAsset;
using CrisesControl.Api.Application.Commands.Assets.GetAssets;
using CrisesControl.Api.Application.Commands.Assets.UpdateAssets;
using CrisesControl.Api.Application.Query;
using MediatR;
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

        public AssetController(IMediator mediator, IAssetQuery assetQuery)
        {
            _mediator = mediator;
            _assetQuery = assetQuery;

        }

        [HttpGet]
        [Route("{CompanyId:int}")]
        public async Task<IActionResult> Index([FromRoute] GetAssetsRequest request, CancellationToken cancellationToken)
        {
            var result = await _assetQuery.GetAssets(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("{CompanyId:int}/{AssetId:int}")]
        public async Task<IActionResult> GetAsset([FromRoute] GetAssetRequest request, CancellationToken cancellationToken)
        {
            var result = await _assetQuery.GetAsset(request, cancellationToken);
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
