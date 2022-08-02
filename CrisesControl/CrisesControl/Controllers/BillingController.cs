using CrisesControl.Api.Application.Commands.Billing.CreateInvoiceSchedule;
using CrisesControl.Api.Application.Commands.Billing.CreateOrder;
using CrisesControl.Api.Application.Commands.Billing.GetAllInvoices;
using CrisesControl.Api.Application.Commands.Billing.GetBillingSummary;
using CrisesControl.Api.Application.Commands.Billing.GetInvoicesById;
using CrisesControl.Api.Application.Commands.Billing.GetInvSchedule;
using CrisesControl.Api.Application.Commands.Billing.GetOrders;
using CrisesControl.Api.Application.Commands.Billing.GetPaymentProfile;
using CrisesControl.Api.Application.Commands.Billing.GetTransactionDetails;
using CrisesControl.Api.Application.Commands.Billing.GetUsageGraph;
using CrisesControl.Api.Application.Commands.Billing.SaveCompanyModules;
using CrisesControl.Api.Application.Commands.Billing.GetUnbilledSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{

    [ApiController]
    [Route("/api/[controller]")]
    [AllowAnonymous]
    public class BillingController : Controller
    {
        private readonly IMediator _mediator;

        public BillingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{CompanyId:int}/PaymentProfile")]
        public async Task<IActionResult> GetPaymentProfile([FromRoute] GetPaymentProfileRequest request, CancellationToken cancellationToken)
        {
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

        [HttpPost]
        [Route("CreateOrder")]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveCompanyModules")]
        public async Task<IActionResult> SaveCompanyModules(SaveCompanyModulesRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("CreateInvoiceSchedule")]
        public async Task<IActionResult> CreateInvoiceSchedule(CreateInvoiceScheduleRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetInvoicesById")]
        public async Task<IActionResult> GetInvoicesById(GetInvoicesByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetTransactionDetails")]
        public async Task<IActionResult> GetTransactionDetails(GetTransactionDetailsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetUsageGraph")]
        public async Task<IActionResult> GetUsageGraph(GetUsageGraphRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("GetUnbilledSummary")]
        public async Task<IActionResult> GetUnbilledSummary(GetUnbilledSummaryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

    }
}
