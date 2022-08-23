using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.GetCompanySOP
{
    public class GetCompanySOPHandler : IRequestHandler<GetCompanySOPRequest, GetCompanySOPResponse>
    {
        private ISopQuery _sopQuery;
        private readonly ILogger<GetCompanySOPHandler> _logger;
        public GetCompanySOPHandler(ISopQuery sopQuery, ILogger<GetCompanySOPHandler> logger)
        {
            _sopQuery = sopQuery;
            _logger = logger;
        }
        public async Task<GetCompanySOPResponse> Handle(GetCompanySOPRequest request, CancellationToken cancellationToken)
        {
            var result = await _sopQuery.GetCompanySOP(request);
            return result;
        }
    }
}
