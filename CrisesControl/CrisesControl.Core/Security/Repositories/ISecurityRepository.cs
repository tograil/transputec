using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CrisesControl.Core.Security
{
    public interface ISecurityRepository
    {
        Task<IEnumerable<CompanySecurityGroup>> GetCompanySecurityGroup(int CompanyID);
        Task<SecurityGroup> GetSecurityGroup(int CompanyID, int SecurityGroupId);
        Task<List<SecurityAllObjects>> GetAllSecurityObjects(int CompanyID);
        Task<bool> CheckMenuAccessAssociation(int SecurityGroupID, int CompanyID);
        Task CreateGroupSecurityObject(int SecurityGroupID, int[] SecurityAdminObjectList);
        Task<int> CreateSecurityGroup(int CompanyId, string Name, string Description, string UserRole, int Status, int CreatedUpdatedBy, string TimeZoneId = "GMT Standard Time");
        Task<bool> DeleteSecurityGroup(int CompanyID, int SecurityGroupId, int CurrentUserId, string TimeZoneId);
        Task<int> UpdateSecurityGroup(int CompanyID, int SecurityGroupId, string GroupName, string GroupDescription, int Status, string UserRole, int[] GroupSecurityObjects, int CurrentUserId, string TimeZoneId);
        Task<int> AddSecurityGroup(int CompanyID, string GroupName, string GroupDescription, int Status, string UserRole, int[] GroupSecurityObjects, int CurrentUserId, string TimeZoneId);
    }
}
