namespace CrisesControl.Api.Application.ViewModels.Department
{
    public class DepartmentInfo
    {
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int CompanyId { get; set; }
        public int Status { get; set; }
    }
}
