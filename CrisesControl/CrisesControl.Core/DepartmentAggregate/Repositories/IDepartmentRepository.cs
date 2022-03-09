using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrisesControl.Core.DepartmentAggregate.Repositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllDepartments();
        Task<Department> GetDepartment(int companyId, int departmentId);
    }
}
