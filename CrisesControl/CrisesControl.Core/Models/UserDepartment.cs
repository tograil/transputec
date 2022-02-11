namespace CrisesControl.Core.Models
{
    public partial class UserDepartment
    {
        public int? UniqueId { get; set; }
        public int CompanyId { get; set; }
        public int? UserId { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int DepartmentStatus { get; set; }
        public int? UserStatus { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
    }
}
