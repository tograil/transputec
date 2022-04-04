using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.DeleteDepartment
{
    public class DeleteDepartmentRequest : IRequest<DeleteDepartmentResponse>
    {
        public int DepartmentId { get; set; }
    }
}
