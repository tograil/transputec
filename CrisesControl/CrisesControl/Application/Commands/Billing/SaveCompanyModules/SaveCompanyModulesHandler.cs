using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Commands.Billing.CreateOrder;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Billing.Repositories;
using CrisesControl.Core.Import;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.SaveCompanyModules
{
    public class SaveCompanyModulesHandler : IRequestHandler<SaveCompanyModulesRequest, SaveCompanyModulesResponse>
    {
        private readonly IBillingRepository _billingRepository;
        private readonly IMapper _mapper;
        public SaveCompanyModulesHandler(IBillingRepository billingRepository, IMapper mapper)
        {
            _billingRepository = billingRepository;
            _mapper = mapper;
        }

        public async Task<SaveCompanyModulesResponse> Handle(SaveCompanyModulesRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SaveCompanyModulesRequest));
            var response = new SaveCompanyModulesResponse();
            var mappedRequest = _mapper.Map<SaveCompanyModulesRequest, OrderModel>(request);
            var result = await _billingRepository.CreateOrder(mappedRequest);
            response.OrderId = result;
            if (result <= 0)
            {
                SaveCompanyModulesResponse DTO = new SaveCompanyModulesResponse()
                {
                    ErrorCode = "E137",
                    ErrorId = 137,
                    Message = "Order was not created"
                };
                return DTO;
            }
            else
            {
                return response;
            }
        }
    }
}
