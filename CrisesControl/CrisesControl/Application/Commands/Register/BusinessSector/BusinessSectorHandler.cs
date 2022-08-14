using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.BusinessSector
{
    public class BusinessSectorHandler : IRequestHandler<BusinessSectorRequest, BusinessSectorResponse>
    {
        private readonly IRegisterQuery _registerQuery;
        private readonly ILogger<BusinessSectorHandler> _logger;
        public BusinessSectorHandler(IRegisterQuery registerQuery, ILogger<BusinessSectorHandler> logger)
        {
           this._registerQuery = registerQuery;
            this._logger = logger;
        }
        public async Task<BusinessSectorResponse> Handle(BusinessSectorRequest request, CancellationToken cancellationToken)
        {
            var result = await _registerQuery.BusinessSector(request);
            return result;
        }
    }
}
