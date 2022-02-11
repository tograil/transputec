namespace CrisesControl.Core.Models
{
    public partial class SmsTriggerUser
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public int Status { get; set; }
    }
}
