using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.DepartmentAggregate.Repositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllDepartments(int companyId);
        Task<Department> GetDepartment(int companyId, int departmentId);
        Task<int> CreateDepartment(Department department, CancellationToken token);
        Task<int> UpdateDepartment(Department department, CancellationToken token);
        Task<int> DeleteDepartment(int departmentId, CancellationToken token);
        bool CheckDuplicate(Department department);
    }
}
