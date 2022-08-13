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
using CrisesControl.Api.Application.Query;

namespace CrisesControl.Api.Controllers
{

    [ApiController]
    [Route("/api/[controller]")]
    [AllowAnonymous]
    public class BillingController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IBillingQuery _billingQuery;

        public BillingController(IMediator mediator, IBillingQuery billingQuery)
        {
            _mediator = mediator;
            _billingQuery = billingQuery;
        }
        /// <summary>
        /// Get payment profile information
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{CompanyId:int}/PaymentProfile")]
        public async Task<IActionResult> GetPaymentProfile([FromRoute] GetPaymentProfileRequest request)
        {
            var result = await _billingQuery.GetPaymentProfile(request);
            return Ok(result);
        }
        /// <summary>
        /// Get the summary information about billing
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBillingSummary")]
        public async Task<IActionResult> GetBillingSummary(GetBillingSummaryRequest request)
        {
            var result = await _billingQuery.GetBillingSummary(request);
            return Ok(result);
        }
        /// <summary>
        /// Get the list of all invoices
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllInvoices")]
        public async Task<IActionResult> GetAllInvoices(GetAllInvoicesRequest request)
        {
            var result = await _billingQuery.GetAllInvoices(request);
            return Ok(result);
        }
        /// <summary>
        /// Get the schedule of invoices
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetInvSchedule")]
        public async Task<IActionResult> GetInvSchedule(GetInvScheduleRequest request, CancellationToken cancellationToken)
        {
            var result = await _billingQuery.GetInvSchedule(request);
            return Ok(result);
        }
        /// <summary>
        /// Get list of orders
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOrders")]
        public async Task<IActionResult> GetOrders(GetOrdersRequest request, CancellationToken cancellationToken)
        {
            var result = await _billingQuery.GetOrders(request);
            return Ok(result);
        }
        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateOrder")]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Save new company modules
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveCompanyModules")]
        public async Task<IActionResult> SaveCompanyModules(SaveCompanyModulesRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Create new invoice schedule
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateInvoiceSchedule")]
        public async Task<IActionResult> CreateInvoiceSchedule(CreateInvoiceScheduleRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Get list of invoice by id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetInvoicesById")]
        public async Task<IActionResult> GetInvoicesById(GetInvoicesByIdRequest request)
        {
            var result = await _billingQuery.GetInvoicesById(request);
            return Ok(result);
        }
        /// <summary>
        /// Get the transaction detail for invoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTransactionDetails")]
        public async Task<IActionResult> GetTransactionDetails(GetTransactionDetailsRequest request)
        {
            var result = await _billingQuery.GetTransactionDetails(request);
            return Ok(result);
        }
        /// <summary>
        /// Get usage graph
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUsageGraph")]
        public async Task<IActionResult> GetUsageGraph(GetUsageGraphRequest request)
        {
            var result = await _billingQuery.GetUsageGraph(request);
            return Ok(result);
        }
        /// <summary>
        /// Get the summary of unbilled invoices
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUnbilledSummary")]
        public async Task<IActionResult> GetUnbilledSummary(GetUnbilledSummaryRequest request)
        {
            var result = await _billingQuery.GetUnbilledSummary(request);
            return Ok(result);
        }

    }
}
