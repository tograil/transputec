using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.GetDepartments
{
    public class GetDepartmentsRequest : IRequest<GetDepartmentsResponse>
    {
        public int CompanyId { get; set; }
    }
}
