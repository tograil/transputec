using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.DepartmentStatus
{
    public class DepartmentStatusRequest:IRequest<DepartmentStatusResponse>
    {
        public int OutUserCompanyId { get; set; }
    }
}
