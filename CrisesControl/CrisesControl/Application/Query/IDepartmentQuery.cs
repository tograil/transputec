//using CrisesControl.Api.Application.Commands.Departments.CreateDepartment;
using CrisesControl.Api.Application.Commands.Departments.GetDepartment;
using CrisesControl.Api.Application.Commands.Departments.GetDepartments;
using CrisesControl.Api.Application.ViewModels.Department;
using CrisesControl.Core.DepartmentAggregate;

namespace CrisesControl.Api.Application.Query
{
    public interface IDepartmentQuery
    {
        public Task<GetDepartmentsResponse> GetDepartments(GetDepartmentsRequest request);
        public Task<GetDepartmentResponse> GetDepartment(GetDepartmentRequest request);

    }
}
