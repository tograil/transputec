using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.CreateDepartment
{
    public class CreateDepartmentRequest : IRequest<CreateDepartmentResponse>
    {
        public int CompanyId { get; set; }
        public string? DepartmentName { get; set; }

    }
}
