using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrisesControl.Core.Departments.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Department = CrisesControl.Core.Departments.Department;

namespace CrisesControl.Infrastructure.Repositories
{
    public class DepartmentRepository: IDepartmentRepository
    {
        private readonly CrisesControlContext _context;

        public DepartmentRepository(CrisesControlContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllDepartments()
        {
            return await _context.Set<Department>().AsNoTracking().ToListAsync();
        }

        public async Task<Department> GetDepartment(int companyId, int departmentId)
        {
            return await _context.Set<Department>().Where(t => t.CompanyId ==companyId && t.DepartmentId == departmentId).AsNoTracking().FirstOrDefaultAsync();
        }
    }
}
