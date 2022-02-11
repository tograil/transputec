namespace CrisesControl.Core.Models
{
    public partial class PublicAlertUser
    {
        public int UserListId { get; set; }
        public int ListId { get; set; }
        public string? EmailId { get; set; }
        public string? MobileNo { get; set; }
    }
}
