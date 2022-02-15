using System.Collections.Generic;
using System.Threading.Tasks;
using DepartmentModel = CrisesControl.Core.Models.Department;

namespace CrisesControl.Core.DepartmentAggregate.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentModel>> GetAllDepartments();
    }
}
