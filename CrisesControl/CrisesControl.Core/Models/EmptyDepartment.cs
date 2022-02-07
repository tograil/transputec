namespace CrisesControl.Core.Models
{
    public partial class EmptyDepartment
    {
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int DepartmentStatus { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
    }
}
