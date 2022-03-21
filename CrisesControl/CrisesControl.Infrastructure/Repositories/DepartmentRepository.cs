﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

            var result = _context.Set<Department>().Where(t=>t.DepartmentId == department.DepartmentId).FirstOrDefault();

            if (result == null)
            {
                return default;
            }
            else
            {
                result.DepartmentName = department.DepartmentName;
                result.Status = department.Status;
                result.UpdatedOn = department.UpdatedOn;
                result.UpdatedBy = department.UpdatedBy;
                await _context.SaveChangesAsync(token);
                return result.DepartmentId;
            }
        }

        public bool CheckDuplicate(Department department)
        {
            return _context.Set<Department>().Where(t => t.DepartmentName.Equals(department.DepartmentName) && t.CompanyId == department.CompanyId).Any();
        }
    }
}
