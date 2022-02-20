using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.CompanyAggregate.Repositories;
using MediatR;

namespace CrisesControl.Core.CompanyAggregate.Handlers.GetCompanyList;

public class GetCompanyListHandler : IRequestHandler<GetCompanyListRequest, GetCompanyListResponse>
{
    private readonly ICompanyRepository _companyService;

    public GetCompanyListHandler(ICompanyRepository companyService)
    {
        _companyService = companyService;
    }

    public async Task<GetCompanyListResponse> Handle(GetCompanyListRequest request, CancellationToken cancellationToken)
    {
        return new GetCompanyListResponse
        {
            Companies = await _companyService.GetAllCompanyList(request.Status, request.CompanyProfile)
        };
    }
}