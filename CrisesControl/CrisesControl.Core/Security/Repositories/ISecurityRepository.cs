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
        Task<IEnumerable<CompanySecurityGroup>> GetCompanySecurityGroup(int companyID);
        Task<SecurityGroups> GetSecurityGroup(int companyID, int securityGroupId);
        Task<List<SecurityAllObjects>> GetAllSecurityObjects(int companyID);
        Task<bool> CheckMenuAccessAssociation(int securityGroupID, int companyID);
        Task CreateGroupSecurityObject(int securityGroupID, int[] securityAdminObjectList);
        Task<int> CreateSecurityGroup(int companyId, string name, string description, string userRole, int status, int createdUpdatedBy, string timeZoneId = "GMT Standard Time");
        Task<bool> DeleteSecurityGroup(int companyID, int securityGroupId, int currentUserId, string timeZoneId);
        Task<int> UpdateSecurityGroup(int companyID, int securityGroupId, string groupName, string groupDescription, int status, string userRole, int[] groupSecurityObjects, int currentUserId, string timeZoneId);
        Task<int> AddSecurityGroup(int companyID, string groupName, string groupDescription, int status, string userRole, int[] groupSecurityObjects, int currentUserId, string timeZoneId);
    }
}
