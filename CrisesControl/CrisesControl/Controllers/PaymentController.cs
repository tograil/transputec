using CrisesControl.Api.Application.Commands.Payments;
using CrisesControl.Api.Application.Commands.Payments.AddRemoveModule;
using CrisesControl.Api.Application.Commands.Payments.GetCompanyPackageItems;
using CrisesControl.Api.Application.Commands.Payments.GetPackageAddons;
using CrisesControl.Api.Application.Commands.Payments.UpdateCompanyPaymentProfile;
using CrisesControl.Api.Application.Commands.Payments.UpgradeByKey;
using CrisesControl.Api.Application.Commands.Payments.UpgradePackage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpPut]
        [Route("UpgradeByKey")]
        public async Task<IActionResult> UpgradeByKey([FromBody] UpgradeByKeyRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateCompanyPaymentProfile([FromBody] UpdateCompanyPaymentProfileRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetCompanyPackageItems([FromRoute] GetCompanyPackageItemsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetPackageAddons([FromRoute] GetPackageAddonsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpgradePackage([FromBody] UpgradePackageRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddRemoveModule([FromRoute] AddRemoveModuleRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        
    }
}
