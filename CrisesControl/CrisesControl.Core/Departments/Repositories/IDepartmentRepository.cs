using CrisesControl.Core.Groups;
using CrisesControl.SharedKernel.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Departments.Repositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllDepartments(int companyId);
        Task<Department> GetDepartment(int companyId, int departmentId);
        Task<int> CreateDepartment(Department department, CancellationToken token);
        Task<int> UpdateDepartment(Department department, CancellationToken token);
        Task<int> DeleteDepartment(int departmentId, CancellationToken token);
        bool CheckDuplicate(Department department);
        bool CheckForExistance(int departmentId);
        Task<bool> UpdateSegregationLink(int SourceID, int TargetID, string Action, GroupType LinkType, int CompanyId);
        Task<List<GroupLink>> SegregationLinks(int TargetID, string MemberShipType, string LinkType, int CurrentUserId, int OutUserCompanyId);
        Task<int> DepartmentStatus(int CompanyID);
        Task CreateSegregtionLink(int sourceID, int targetID, GroupType LinkType, int companyId);

    }
}
