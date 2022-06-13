using CrisesControl.Core.Users;

namespace CrisesControl.Core.Register
{
    public class CompanyUser
    {
        public int UserId { get; set; }
        public UserFullName UserName { get; set; }
        public string UserEmail { get; set; }
        public string? UniqueID { get; set; }
        public int CompanyId { get; set; }
        public string TimeZoneId { get; set; }
    }
}
