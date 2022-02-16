using MediatR;

namespace CrisesControl.Core.CompanyAggregate.Handlers.GetCompanyList;

public class GetCompanyListRequest : IRequest<GetCompanyListResponse>
{
    public int? Status { get; set; }
    public string? CompanyProfile { get; set; }
}