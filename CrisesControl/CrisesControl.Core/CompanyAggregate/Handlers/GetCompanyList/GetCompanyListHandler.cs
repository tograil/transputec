using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.CompanyAggregate.Services;
using MediatR;

namespace CrisesControl.Core.CompanyAggregate.Handlers.GetCompanyList;

public class GetCompanyListHandler : IRequestHandler<GetCompanyListRequest, GetCompanyListResponse>
{
    private readonly ICompanyService _companyService;

    public GetCompanyListHandler(ICompanyService companyService)
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