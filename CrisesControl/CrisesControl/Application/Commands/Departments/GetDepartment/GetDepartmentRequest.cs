using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.GetDepartment
{
    public class GetDepartmentRequest : IRequest<GetDepartmentResponse>
    {
        public int DepartmentId { get; set; }  
        public int CompanyId { get; set; }
        public string? DepartmentName { get; set;}
    }
}
