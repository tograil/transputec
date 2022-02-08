namespace CrisesControl.Core.Models
{
    public partial class UsersSearch
    {
        public int UserId { get; set; }
        public byte[]? SearchFieldEncrypted { get; set; }
    }
}
