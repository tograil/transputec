using CrisesControl.Core.DepartmentAggregate.Handles.GetDepartment;
using CrisesControl.Core.DepartmentAggregate.Services;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public class DepartmentService: IDepartmentService
    {
        private readonly CrisesControlContext _context;

        public DepartmentService(CrisesControlContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllDepartments()
        {
            return await _context.Set<Department>().AsNoTracking().ToListAsync();
        }

        public async Task<Department> GetDepartment(GetDepartmentRequest departmentRequest)
        {
            return await _context.Set<Department>().Where(t => t.CompanyId == departmentRequest.CompanyId && t.DepartmentId == departmentRequest.DepartmentId).AsNoTracking().FirstOrDefaultAsync();
        }
    }
}
