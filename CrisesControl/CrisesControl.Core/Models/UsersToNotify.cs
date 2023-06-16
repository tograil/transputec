using CrisesControl.Core.Users;

namespace CrisesControl.Core.Models
{
    public partial class UsersToNotify
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public int? ActiveIncidentId { get; set; }
        public User User { get; set; }
    }
}
