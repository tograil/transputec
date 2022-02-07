namespace CrisesControl.Core.Models
{
    public partial class Lookup
    {
        public int LookupId { get; set; }
        public string? Category { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public int CategoryId { get; set; }
    }
}
