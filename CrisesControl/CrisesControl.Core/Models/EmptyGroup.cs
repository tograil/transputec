namespace CrisesControl.Core.Models
{
    public partial class EmptyGroup
    {
        public int CompanyId { get; set; }
        public int GroupId { get; set; }
        public string? GroupName { get; set; }
        public int GroupStatus { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
    }
}
