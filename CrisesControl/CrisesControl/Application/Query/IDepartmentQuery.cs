using CrisesControl.Api.Application.Commands.Departments.CreateDepartment;
using CrisesControl.Api.Application.Commands.Departments.GetDepartment;
using CrisesControl.Api.Application.Commands.Departments.GetDepartments;
using CrisesControl.Api.Application.ViewModels.Department;
using CrisesControl.Core.DepartmentAggregate;

namespace CrisesControl.Api.Application.Query
{
    public interface IDepartmentQuery
    {
        public Task<IEnumerable<DepartmentInfo>> GetDepartments(GetDepartmentsRequest request);
        public Task<DepartmentInfo> GetDepartment(GetDepartmentRequest request);

    }
}
