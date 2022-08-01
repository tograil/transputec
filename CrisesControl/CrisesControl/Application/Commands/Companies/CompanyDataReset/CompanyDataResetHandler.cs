using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.CompanyDataReset
{
    public class CompanyDataResetHandler : IRequestHandler<CompanyDataResetRequest, CompanyDataResetResponse>
    {
        private readonly ICompanyQuery _companyQuery;
        private readonly ILogger<CompanyDataResetHandler> _logger;
        public CompanyDataResetHandler(ICompanyQuery companyQuery, ILogger<CompanyDataResetHandler> logger)
        {
            this._companyQuery = companyQuery;
            this._logger = logger;
           
        }
        public async Task<CompanyDataResetResponse> Handle(CompanyDataResetRequest request, CancellationToken cancellationToken)
        {
            var results = await _companyQuery.CompanyDataReset(request);
            return results;
        }
    }
}
