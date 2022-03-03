using CrisesControl.Api.Application.Commands.Departments.GetDepartment;
using CrisesControl.Api.Application.Commands.Departments.GetDepartments;
using CrisesControl.Api.Application.ViewModels.Department;
using CrisesControl.Core.DepartmentAggregate;
using CrisesControl.Core.DepartmentAggregate.Repositories;

namespace CrisesControl.Api.Application.Query
{
    public class DepartmentQuery : IDepartmentQuery
    {
        private readonly IDepartmentRepository _departmentRepository;
        public DepartmentQuery(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<DepartmentInfo>> GetDepartments(GetDepartmentsRequest request)
        {
            var departments = await _departmentRepository.GetAllDepartments(request.CompanyId);
            return departments.Select(x => new DepartmentInfo
            {
                DepartmentId = x.DepartmentId,
                DepartmentName = x.DepartmentName,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                UpdatedBy = x.UpdatedBy,
                CompanyId = x.CompanyId,
                Status = x.Status
            });
        }

        public async Task<DepartmentInfo> GetDepartment(GetDepartmentRequest request)
        {
            var department = await _departmentRepository.GetDepartment(request.CompanyId, request.DepartmentId);
            return new DepartmentInfo
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                CreatedBy = department.CreatedBy,
                CreatedOn = department.CreatedOn,
                UpdatedBy = department.UpdatedBy,
                CompanyId = department.CompanyId,
                Status = department.Status
            };
        }
    }
}
