using CrisesControl.Api.Maintenance.Interfaces;

namespace CrisesControl.Api.Maintenance
{
    public class Paging : IPaging
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string Search { get; set; }
        public string? Order { get; set; }
        public string? Dir { get; set; }
        public string? UniqueKey { get; set; }
        public string? Filters { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; } = string.Empty;
        public bool Apply { get; set; }
    }
}