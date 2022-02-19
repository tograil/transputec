using System.Collections.Generic;

namespace CrisesControl.Core.CompanyAggregate.Handlers.GetCompanyList;

public class GetCompanyListResponse
{
    public IEnumerable<CompanyRoot> Companies { get; set; }
}