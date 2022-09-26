using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Models;
using CrisesControl.Core.Security;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class SecurityRepository : ISecurityRepository
    {
        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SecurityRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }
        /* Created new store procedure called GetCompanySecurityGroup
          *
          *
          *              ALTER PROCEDURE[dbo].[GetCompanySecurityGroup]
          *                      @CompanyID INT
          *              AS
          *              BEGIN
          *                  SET NOCOUNT ON;
          *                  SELECT DISTINCT SG.SecurityGroupId,
          *                                  SG.CompanyId,
          *                                  SG.Name,
          *                                  SG.Description,
          *                                  SG.Status,
          *                                  SG.UserRole
          *                  FROM   SecurityGroup AS SG WITH(NOLOCK)
          *                         INNER JOIN
          *                         dbo.Company AS Co WITH(NOLOCK)
          *                         ON Co.CompanyId = SG.CompanyId
          *                         LEFT OUTER JOIN
          *                         (SELECT DISTINCT SO.SecurityObjectID,
          *                                          SO.ParentID,
          *                                          SO.TypeID,
          *                                          SO.SecurityKey,
          *                                          SO.Name,
          *                                          SO.Status,
          *                                          GSO.SecurityGroupId
          *                          FROM   dbo.SecurityObjects AS SO WITH (NOLOCK)
          *                                 INNER JOIN
          *                                 dbo.GroupSecuityObjects AS GSO WITH (NOLOCK)
          *                                 ON GSO.SecurityObjectID = SO.SecurityObjectID
          *                                 INNER JOIN
          *                                 dbo.CompanyPackageFeature AS PF WITH (NOLOCK)
          *                                 ON SO.SecurityObjectID = PF.SecurityObjectID
          *                          WHERE PF.CompanyID = @CompanyID) AS ASG
          *                         ON ASG.SecurityGroupId = SG.SecurityGroupId
          *                  WHERE  SG.CompanyId = @CompanyID;
          *                      END
          *
          *
          *
          *
          *
          *
          */
        public async Task<IEnumerable<CompanySecurityGroup>> GetCompanySecurityGroup(int companyID)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", companyID);
                return await _context.Set<CompanySecurityGroup>().FromSqlRaw("EXEC GetCompanySecurityGroup @CompanyID", pCompanyID).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<SecurityGroups> GetSecurityGroup(int companyID, int securityGroupId)
        {
            
            try
            {
                var pSecurityGroupID  = new SqlParameter("@SecurityGroupID", securityGroupId );
                var pCompanyID = new SqlParameter("@CompanyID", companyID);

                var SecurityInfo = _context.Set<SecurityGroups>().FromSqlRaw("exec Pro_Get_SecurityGroup @CompanyID, @SecurityGroupID", pCompanyID, pSecurityGroupID).AsEnumerable();
             
                if (SecurityInfo != null)
                {
                    return SecurityInfo.FirstOrDefault();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<SecurityAllObjects>> GetAllSecurityObjects(int companyID)
        {
          
            try
            {
                var companyId = new SqlParameter("@CompanyID", companyID);
                var SecurityObjList = await _context.Set<SecurityAllObjects>().FromSqlRaw("exec Pro_Get_ALL_Security_Objects @CompanyID", companyId).ToListAsync();

                if (SecurityObjList != null)
                {
                    return SecurityObjList;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AddSecurityGroup(int companyID, string groupName, string groupDescription, int status, string userRole, int[] groupSecurityObjects, int currentUserId, string timeZoneId)
        {
          
            try
            {
                SecurityGroup secGroup = new SecurityGroup()
                {
                    CompanyId = companyID,
                    Name = groupName,
                    Description = groupDescription,
                    Status = status,
                    UserRole = userRole,
                    CreatedBy = currentUserId,
                    CreatedOn = DateTime.Now,
                    UpdatedBy = currentUserId,
                    UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId)
                };
                await _context.AddAsync(secGroup);
                await _context.SaveChangesAsync();


                if (groupSecurityObjects != null)
                {
                    foreach (int SecObj in groupSecurityObjects)
                    {
                        GroupSecuityObject SecItem = new GroupSecuityObject()
                        {
                            SecurityGroupId = secGroup.SecurityGroupId,
                            SecurityObjectId = Convert.ToInt32(SecObj)
                        };
                        await _context.AddAsync(SecItem);
                        await _context.SaveChangesAsync();
                    }
                }

                return secGroup.SecurityGroupId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UpdateSecurityGroup(int companyID, int securityGroupId, string groupName, string groupDescription, int status, string userRole, int[] groupSecurityObjects, int currentUserId, string timeZoneId)
        {
            
            try
            {
                var GroupData = await  _context.Set<SecurityGroup>()
                                 .Where(GRP=> GRP.CompanyId == companyID && GRP.SecurityGroupId == securityGroupId).FirstOrDefaultAsync();
                if (GroupData != null)
                {
                    GroupData.Name = groupName;
                    GroupData.Description = groupDescription;
                    GroupData.UserRole = userRole;
                    GroupData.Status = status;
                    GroupData.UpdatedBy = currentUserId;
                    GroupData.UpdatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(timeZoneId, System.DateTime.Now);
                };
               _context.Update(GroupData);
                await _context.SaveChangesAsync();

                var RegGrpDel = await  _context.Set<GroupSecuityObject>()
                                .Where(GRPITM=> GRPITM.SecurityGroupId == securityGroupId).ToListAsync();

                List<int[]> GSOList = new List<int[]>();
                if (groupSecurityObjects != null)
                {
                    if (groupSecurityObjects.Length > 0)
                    {
                        foreach (int SecObj in groupSecurityObjects)
                        {
                            var ISExist = RegGrpDel.FirstOrDefault(s => s.SecurityGroupId == securityGroupId && s.SecurityObjectId == SecObj);
                            if (ISExist == null)
                            {
                                GroupSecuityObject SecItem = new GroupSecuityObject()
                                {
                                    SecurityGroupId = securityGroupId,
                                    SecurityObjectId = Convert.ToInt32(SecObj)
                                };
                                await _context.AddAsync(SecItem);
                            }
                            else
                            {
                                int[] Arr = new int[2];
                                Arr[0] = ISExist.SecurityGroupId;
                                Arr[1] = ISExist.SecurityObjectId;
                                GSOList.Add(Arr);
                            }
                        }
                        await _context.SaveChangesAsync();
                        foreach (var Ditem in RegGrpDel)
                        {
                            bool ISDEL = GSOList.Any(s => s[0] == Ditem.SecurityGroupId && s[1] == Ditem.SecurityObjectId);
                            if (ISDEL == false)
                            {
                                _context.Remove(Ditem);
                                await _context.SaveChangesAsync(); 
                            }
                        }
                    }
                }
                if (status == 0)
                {
                    var delUserSecurityGroup = await  _context.Set<UserSecurityGroup>().Where(USG=> USG.SecurityGroupId == securityGroupId).ToListAsync();
                    _context.RemoveRange(delUserSecurityGroup);
                    await _context.SaveChangesAsync();
                }
                return securityGroupId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteSecurityGroup(int companyID, int securityGroupId, int currentUserId, string timeZoneId)
        {

           
            try
            {
                var checkGroup = CheckMenuAccessAssociation(securityGroupId, companyID);
                //if (checkGroup)
                //{
                //    ResultDTO.ErrorId = 230;
                //    string Message = "Security group attached to user, cannot be deleted.";
                //    return ResultDTO;
                //}

                var GroupData = await  _context.Set<SecurityGroup>().
                                 Where(GRP=> GRP.CompanyId == companyID && GRP.SecurityGroupId == securityGroupId).FirstOrDefaultAsync();
                if (GroupData != null)
                {
                    GroupData.Status = 3;
                    GroupData.Name = "DEL_" + GroupData.Name;
                    GroupData.UpdatedBy = currentUserId;
                    GroupData.UpdatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(timeZoneId, DateTime.Now);
                    _context.Update(GroupData);
                    await _context.SaveChangesAsync();

                    //var delUserSecurityGroup = (from USG in db.UserSecurityGroup where USG.SecurityGroupId == SecurityGroupId select USG).ToList();
                    //db.UserSecurityGroup.RemoveRange(delUserSecurityGroup);
                    //db.SaveChanges(CurrentUserId, CompanyID);
                    return true;
                }
                return false;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<int> CreateSecurityGroup(int companyId, string name, string description, string userRole, int status, int createdUpdatedBy, string timeZoneId = "GMT Standard Time")
        {
            try
            {
                var chkdupe =await _context.Set<SecurityGroup>().Where(SC=> SC.CompanyId == companyId && SC.Name == name).AnyAsync();
                if (!chkdupe)
                {
                    SecurityGroup NewSecurityGroup = new SecurityGroup();
                    NewSecurityGroup.CompanyId = companyId;
                    NewSecurityGroup.Name = name.Left( 50);
                    NewSecurityGroup.Description = description;
                    NewSecurityGroup.UserRole = userRole;
                    NewSecurityGroup.Status = status;
                    NewSecurityGroup.CreatedBy = createdUpdatedBy;
                    NewSecurityGroup.CreatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);
                    NewSecurityGroup.UpdatedBy = createdUpdatedBy;
                    NewSecurityGroup.UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);
                    await _context.AddAsync(NewSecurityGroup);
                    await _context.SaveChangesAsync();
                    return NewSecurityGroup.SecurityGroupId;
                }
                return 0;
            }
            catch (Exception ex) {
                throw ex; 
            }
           
        }

        public async Task CreateGroupSecurityObject(int securityGroupID, int[] securityAdminObjectList)
        {
            foreach (var AdminSecurityItem in securityAdminObjectList)
            {
                GroupSecuityObject groupSecurityObjects = new GroupSecuityObject()
                {
                    SecurityObjectId = AdminSecurityItem,
                    SecurityGroupId = securityGroupID,
                };
                await _context.AddAsync(groupSecurityObjects);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckMenuAccessAssociation(int securityGroupID, int companyID)
        {
            try
            {
                DBCommon DBC = new DBCommon(_context,_httpContextAccessor);
                bool sendemail = false;

                var pCompanyId = new SqlParameter("@CompanyId", companyID);
                var pSecurityGroupID = new SqlParameter("@SecurityGroupID", securityGroupID);

                var result = await _context.Set<ModuleLinks>().FromSqlRaw("exec Pro_Get_SecurityGroup_Association @SecurityGroupID, @CompanyID", pSecurityGroupID, pCompanyId).ToListAsync();

                if (result.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<table width=\"100%\" class=\"user_list\" style=\"border:1px solid #000000;border-collapse: collapse\"><tr width=\"50%\"><th>User Email</th><th width=\"50%\">User Name</th></tr>");
                    sendemail = true;
                    foreach (var item in result)
                    {
                        sb.AppendLine("<tr><td>" + item.ModuleItem + "</td><td>" + DBC.UserName(item.UserName) + "</td></tr>");
                    }
                    sb.AppendLine("</table>");
                    SendEmail SDE = new SendEmail(_context, DBC);
                    await SDE.SendMenuAccessAssociationsToAdmin(sb.ToString(), securityGroupID, companyID);
                }

                return sendemail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
    }
}
