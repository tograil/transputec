using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrisesControl.Core.Departments.Repositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllDepartments();
        Task<Department> GetDepartment(int companyId, int departmentId);
    }
}
