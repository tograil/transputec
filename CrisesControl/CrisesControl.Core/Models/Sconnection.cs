namespace CrisesControl.Core.Models
{
    public partial class Sconnection
    {
        public int UserConnectionId { get; set; }
        public string? ConnectionId { get; set; }
        public string? UserAgent { get; set; }
        public bool Connected { get; set; }
        public int? UsersUserId { get; set; }
    }
}
