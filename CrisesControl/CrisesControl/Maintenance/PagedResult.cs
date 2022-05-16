namespace CrisesControl.Api.Maintenance
{
    public class PagedResult
    {
        public IEnumerable<dynamic> ListToShow { get; set; } = new List<dynamic>();
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}