using CrisesControl.Core.Paging;

namespace CrisesControl.Api.Application.Commands.Common;

public class PagedRequest
{
    // properties are not capital due to json mapping
    public int draw { get; set; }
    public int start { get; set; }
    public int length { get; set; }
    public Search? search { get; set; }
    public List<Order>? order { get; set; }
}