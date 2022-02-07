namespace CrisesControl.Core.Models
{
    public partial class EmptyLocation
    {
        public int CompanyId { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public string? Lat { get; set; }
        public string? Long { get; set; }
        public string? Desc { get; set; }
        public string? PostCode { get; set; }
        public int LocationStatus { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
    }
}
