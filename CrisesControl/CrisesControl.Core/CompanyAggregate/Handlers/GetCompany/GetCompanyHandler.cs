using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace CrisesControl.Core.CompanyAggregate.Handlers.GetCompany
{
    public class GetCompanyHandler : IRequestHandler<GetCompanyRequest, GetCompanyResponse>
    {
        public Task<GetCompanyResponse> Handle(GetCompanyRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new GetCompanyResponse());
        }
    }
}