using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.UpdateDepartment
{
    public class UpdateDepartmentRequest : IRequest<UpdateDepartmentResponse>
    {
        public int DepartmentId { get; set; }  
        public int CompanyId { get; set; }
        public string? DepartmentName { get; set; }
    }
}
