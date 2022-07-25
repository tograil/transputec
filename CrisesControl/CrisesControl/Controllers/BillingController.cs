using CrisesControl.Api.Application.Commands.Billing.GetAllInvoices;
using CrisesControl.Api.Application.Commands.Billing.GetBillingSummary;
using CrisesControl.Api.Application.Commands.Billing.GetInvSchedule;
using CrisesControl.Api.Application.Commands.Billing.GetOrders;
using CrisesControl.Api.Application.Commands.Billing.GetPaymentProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers {

    [ApiController]
    [Route("/api/[controller]")]
    [AllowAnonymous]
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

        [HttpPost]
        [Route("GetBillingSummary")]
        public async Task<IActionResult> GetBillingSummary(GetBillingSummaryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetAllInvoices")]
        public async Task<IActionResult> GetAllInvoices(GetAllInvoicesRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetInvSchedule")]
        public async Task<IActionResult> GetInvSchedule(GetInvScheduleRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetOrders")]
        public async Task<IActionResult> GetOrders(GetOrdersRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

    }
}
