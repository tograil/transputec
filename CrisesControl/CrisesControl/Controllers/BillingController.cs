using CrisesControl.Api.Application.Commands.Billing.GetPaymentProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers {

    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class BillingController : Controller {
        private readonly IMediator _mediator;

        public BillingController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{CompanyId:int}/PaymentProfile")]
        public async Task<IActionResult> GetPaymentProfile([FromRoute] GetPaymentProfileRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

    }
}
