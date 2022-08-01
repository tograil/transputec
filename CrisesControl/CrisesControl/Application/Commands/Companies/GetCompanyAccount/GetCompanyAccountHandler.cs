using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetCompanyAccount
{
    public class GetCompanyAccountHandler : IRequestHandler<GetCompanyAccountRequest, GetCompanyAccountResponse>
    {
        private readonly ICompanyQuery _companyQuery;
        private readonly ILogger<GetCompanyAccountHandler> _logger;
        public GetCompanyAccountHandler(ICompanyQuery companyQuery, ILogger<GetCompanyAccountHandler> logger)
        {
            _companyQuery = companyQuery;
            _logger = logger;
        }

        public async Task<GetCompanyAccountResponse> Handle(GetCompanyAccountRequest request, CancellationToken cancellationToken)
        {
            var result = await _companyQuery.GetCompanyAccount(request);
            return result;
        }
    }
}
