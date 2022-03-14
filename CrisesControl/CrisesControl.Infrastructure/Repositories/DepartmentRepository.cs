using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.DepartmentAggregate.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Department = CrisesControl.Core.DepartmentAggregate.Department;

namespace CrisesControl.Infrastructure.Repositories
{
    public class DepartmentRepository: IDepartmentRepository
    {
        private readonly CrisesControlContext _context;

        public DepartmentRepository(CrisesControlContext context)
        {
            _context = context;
        }

        public async Task<int> CreateDepartment(Department department, CancellationToken token)
        {
            await _context.AddAsync(department, token);

            await _context.SaveChangesAsync(token);

            return department.DepartmentId;
        }

        public async Task<int> DeleteDepartment(int departmentId, CancellationToken token)
        {
            await _context.AddAsync(departmentId, token);

            await _context.SaveChangesAsync(token);

            return departmentId;
        }

        public async Task<IEnumerable<Department>> GetAllDepartments(int companyId)
        {
            return await _context.Set<Department>().Where(t=>t.CompanyId == companyId).ToListAsync();
        }

        public async Task<Department> GetDepartment(int companyId, int departmentId)
        {
            return await _context.Set<Department>().Where(t => t.CompanyId == companyId && t.DepartmentId == departmentId).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateDepartment(Department department, CancellationToken token)
        {
            await _context.AddAsync(department, token);

            await _context.SaveChangesAsync(token);

            return department.DepartmentId;
        }
    }
}
