namespace CrisesControl.Core.Models
{
    public partial class UserRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public string RoleCode { get; set; } = null!;
    }
}
