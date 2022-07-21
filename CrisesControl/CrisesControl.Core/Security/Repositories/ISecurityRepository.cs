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
        Task<int> AddSecurityGroup(SecurityGroup securityGroup);
        Task<int> UpdateSecurityGroup(SecurityGroup securityGroup);
        Task<SecurityGroup> GetSecurityGroup(int SecurityGroupId, int CompanyID);
        Task<List<SecurityAllObjects>> GetAllSecurityObjects(int CompanyID);
        Task<List<GroupSecuityObject>> GetGroupSecuityObject(int SecurityGroupId);
        Task<int> AddGroupSecuityObject(GroupSecuityObject GroupSecurityObject);
        Task<int> UpdateGroupSecuityObject(GroupSecuityObject GroupSecurityObject);
        Task DeleteGroupSecuityObject(GroupSecuityObject GroupSecurityObject);
        Task DeleteUserSecurityGroup(List<UserSecurityGroup> userSecurityGroup);
        Task<List<UserSecurityGroup>> GetUserSecurityGroup(int SecurityGroupId);
        Task<bool> CheckMenuAccessAssociation(int SecurityGroupID, int CompanyID);
    }
}
