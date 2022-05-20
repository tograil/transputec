using CrisesControl.Core.Paging;

namespace CrisesControl.Api.Application.Query.Common;

public class PagedRequest
{
    // properties are not capital due to json mapping
    public int Draw { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public Search? Search { get; set; }
    public List<Order>? Order { get; set; }
}