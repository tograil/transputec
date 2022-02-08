namespace CrisesControl.Core.Models
{
    public partial class MailMerge
    {
        public int Id { get; set; }
        public string? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Email2 { get; set; }
    }
}
