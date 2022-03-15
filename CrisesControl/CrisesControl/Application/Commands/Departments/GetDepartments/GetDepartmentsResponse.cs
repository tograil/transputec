using CrisesControl.Api.Application.Commands.Departments.GetDepartment;

namespace CrisesControl.Api.Application.Commands.Departments.GetDepartments
{
    public class GetDepartmentsResponse
    {
        public List<GetDepartmentResponse> Data { get; set; }
    }
}
