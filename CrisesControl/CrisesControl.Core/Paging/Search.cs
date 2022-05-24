namespace CrisesControl.Core.Paging;

public record Search
{
    public string value { get; set; }
    public string regex { get; set; }
}