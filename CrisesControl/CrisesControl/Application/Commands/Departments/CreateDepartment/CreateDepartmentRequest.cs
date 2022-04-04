using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.CreateDepartment
{
    public class CreateDepartmentRequest : IRequest<CreateDepartmentResponse>
    {
        public int CompanyId { get; set; }
        public string? DepartmentName { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }
        public int Status { get; set; }
    }
}
