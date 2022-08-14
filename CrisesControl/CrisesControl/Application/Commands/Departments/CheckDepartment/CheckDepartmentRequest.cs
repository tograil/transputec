using CrisesControl.Core.Departments;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.CheckDepartment
{
    public class CheckDepartmentRequest:IRequest<CheckDepartmentResponse>
    {
        public int DepartmentId { get; set; }
    }
}
