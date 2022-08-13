using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Billing.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.CreateInvoiceSchedule
{
    public class CreateInvoiceScheduleHandler: IRequestHandler<CreateInvoiceScheduleRequest, CreateInvoiceScheduleResponse>
    {
        private readonly IBillingRepository _billingRepository;
        private readonly IMapper _mapper;
        public CreateInvoiceScheduleHandler(IBillingRepository billingRepository, IMapper mapper)
        {
            _billingRepository = billingRepository;
            _mapper = mapper;
        }

        public async Task<CreateInvoiceScheduleResponse> Handle(CreateInvoiceScheduleRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateInvoiceScheduleRequest));

            var response = new CreateInvoiceScheduleResponse();
            var mappedRequest = _mapper.Map<CreateInvoiceScheduleRequest, OrderModel>(request);
            var result = await _billingRepository.CreateInvoiceSchedule(mappedRequest);
            response.OrderId = result;
            return response;
        }
    }
}
