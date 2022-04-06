namespace CrisesControl.Core.Models
{
    public partial class EmptyUser
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public int Status { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
    }
}
