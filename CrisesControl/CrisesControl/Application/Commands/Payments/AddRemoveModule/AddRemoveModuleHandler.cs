using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Payments.AddRemoveModule
{
    public class AddRemoveModuleHandler : IRequestHandler<AddRemoveModuleRequest, AddRemoveModuleResponse>
    {
        private readonly ILogger<AddRemoveModuleHandler> _logger;
        private readonly IPaymentQuery _paymentQuery;
        
        public AddRemoveModuleHandler(ILogger<AddRemoveModuleHandler> logger, IPaymentQuery paymentQuery)
        {
            this._logger = logger;
            this._paymentQuery = paymentQuery;
          
        }
        public async Task<AddRemoveModuleResponse> Handle(AddRemoveModuleRequest request, CancellationToken cancellationToken)
        {
            var result = await _paymentQuery.AddRemoveModule(request);
            return result;
        }
    }
}
