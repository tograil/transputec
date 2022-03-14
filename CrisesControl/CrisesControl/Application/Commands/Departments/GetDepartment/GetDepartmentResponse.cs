namespace CrisesControl.Api.Application.Commands.Departments.GetDepartment
{
    public class GetDepartmentResponse
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
