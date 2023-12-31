﻿using CrisesControl.Core.Groups;
using CrisesControl.SharedKernel.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Departments.Repositories
{
    public interface IDepartmentRepository
    {
        Task<List<DepartmentDetail>> GetAllDepartments(int companyId, bool filterVirtual);
        Task<Department> GetDepartment(int companyId, int departmentId);
        Task<int> CreateDepartment(Department department, CancellationToken token);
        Task<int> UpdateDepartment(Department department, CancellationToken token);
        Task<int> DeleteDepartment(int departmentId, CancellationToken token);
        bool CheckDuplicate(Department department);
        Task<bool> CheckForExistance(int departmentId);
        Task<bool> UpdateSegregationLink(int SourceID, int TargetID,  GroupType LinkType, int CompanyId);
        Task<List<GroupLink>> SegregationLinks(int targetID, string memberShipType, string linkType, int currentUserId, int outUserCompanyId);
        Task<int> DepartmentStatus(int CompanyID);
        Task CreateSegregtionLink(int sourceID, int targetID, GroupType LinkType, int companyId);

    }
}
