using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.CompanyDataReset
{
    public class CompanyHandler : IRequestHandler<CompanyDataResetRequest, CompanyDataResetResponse>
    {
        private readonly ICompanyQuery _companyQuery;
        public CompanyHandler(ICompanyQuery companyQuery)
        {

        }
        public Task<CompanyDataResetResponse> Handle(CompanyDataResetRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
