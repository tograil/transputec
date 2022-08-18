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
        public async Task<IEnumerable<CompanySecurityGroup>> GetCompanySecurityGroup(int CompanyID)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                return await _context.Set<CompanySecurityGroup>().FromSqlRaw("EXEC GetCompanySecurityGroup @CompanyID", pCompanyID).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<SecurityGroup> GetSecurityGroup(int CompanyID, int SecurityGroupId)
        {
            
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pSecurityGroupID = new SqlParameter("@CompanyID", SecurityGroupId);

                var SecurityInfo = await _context.Set<SecurityGroup>().FromSqlRaw("exec Pro_Get_SecurityGroup @CompanyID, @SecurityGroupID", pCompanyID, pSecurityGroupID).FirstOrDefaultAsync();

                if (SecurityInfo != null)
                {
                    return SecurityInfo;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<SecurityAllObjects>> GetAllSecurityObjects(int CompanyID)
        {
          
            try
            {
                var companyId = new SqlParameter("@CompanyID", CompanyID);
                var SecurityObjList = await _context.Set<SecurityAllObjects>().FromSqlRaw("exec Pro_Get_ALL_Security_Objects @CompanyID").ToListAsync();

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
        public async Task<int> AddSecurityGroup(int CompanyID, string GroupName, string GroupDescription, int Status, string UserRole, int[] GroupSecurityObjects, int CurrentUserId, string TimeZoneId)
        {
          
            try
            {
                SecurityGroup secGroup = new SecurityGroup()
                {
                    CompanyId = CompanyID,
                    Name = GroupName,
                    Description = GroupDescription,
                    Status = Status,
                    UserRole = UserRole,
                    CreatedBy = CurrentUserId,
                    CreatedOn = DateTime.Now,
                    UpdatedBy = CurrentUserId,
                    UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId)
                };
                await _context.AddAsync(secGroup);
                await _context.SaveChangesAsync();


                if (GroupSecurityObjects != null)
                {
                    foreach (int SecObj in GroupSecurityObjects)
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

        public async Task<int> UpdateSecurityGroup(int CompanyID, int SecurityGroupId, string GroupName, string GroupDescription, int Status, string UserRole, int[] GroupSecurityObjects, int CurrentUserId, string TimeZoneId)
        {
            
            try
            {
                var GroupData = await  _context.Set<SecurityGroup>()
                                 .Where(GRP=> GRP.CompanyId == CompanyID && GRP.SecurityGroupId == SecurityGroupId).FirstOrDefaultAsync();
                if (GroupData != null)
                {
                    GroupData.Name = GroupName;
                    GroupData.Description = GroupDescription;
                    GroupData.UserRole = UserRole;
                    GroupData.Status = Status;
                    GroupData.UpdatedBy = CurrentUserId;
                    GroupData.UpdatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(TimeZoneId, System.DateTime.Now);
                };
               //_context.Update(GroupData);
                await _context.SaveChangesAsync();

                var RegGrpDel = await  _context.Set<GroupSecuityObject>()
                                .Where(GRPITM=> GRPITM.SecurityGroupId == SecurityGroupId).ToListAsync();

                List<int[]> GSOList = new List<int[]>();
                if (GroupSecurityObjects != null)
                {
                    if (GroupSecurityObjects.Length > 0)
                    {
                        foreach (int SecObj in GroupSecurityObjects)
                        {
                            var ISExist = RegGrpDel.FirstOrDefault(s => s.SecurityGroupId == SecurityGroupId && s.SecurityObjectId == SecObj);
                            if (ISExist == null)
                            {
                                GroupSecuityObject SecItem = new GroupSecuityObject()
                                {
                                    SecurityGroupId = SecurityGroupId,
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
                if (Status == 0)
                {
                    var delUserSecurityGroup = await  _context.Set<UserSecurityGroup>().Where(USG=> USG.SecurityGroupId == SecurityGroupId).ToListAsync();
                    _context.RemoveRange(delUserSecurityGroup);
                    await _context.SaveChangesAsync();
                }
                return SecurityGroupId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteSecurityGroup(int CompanyID, int SecurityGroupId, int CurrentUserId, string TimeZoneId)
        {

           
            try
            {
                var checkGroup = CheckMenuAccessAssociation(SecurityGroupId, CompanyID);
                //if (checkGroup)
                //{
                //    ResultDTO.ErrorId = 230;
                //    string Message = "Security group attached to user, cannot be deleted.";
                //    return ResultDTO;
                //}

                var GroupData = await  _context.Set<SecurityGroup>().
                                 Where(GRP=> GRP.CompanyId == CompanyID && GRP.SecurityGroupId == SecurityGroupId).FirstOrDefaultAsync();
                if (GroupData != null)
                {
                    GroupData.Status = 3;
                    GroupData.Name = "DEL_" + GroupData.Name;
                    GroupData.UpdatedBy = CurrentUserId;
                    GroupData.UpdatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(TimeZoneId, DateTime.Now);
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

        public async Task<int> CreateSecurityGroup(int CompanyId, string Name, string Description, string UserRole, int Status, int CreatedUpdatedBy, string TimeZoneId = "GMT Standard Time")
        {
            try
            {
                var chkdupe =await _context.Set<SecurityGroup>().Where(SC=> SC.CompanyId == CompanyId && SC.Name == Name).AnyAsync();
                if (!chkdupe)
                {
                    SecurityGroup NewSecurityGroup = new SecurityGroup();
                    NewSecurityGroup.CompanyId = CompanyId;
                    NewSecurityGroup.Name = Name.Left( 50);
                    NewSecurityGroup.Description = Description;
                    NewSecurityGroup.UserRole = UserRole;
                    NewSecurityGroup.Status = Status;
                    NewSecurityGroup.CreatedBy = CreatedUpdatedBy;
                    NewSecurityGroup.CreatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    NewSecurityGroup.UpdatedBy = CreatedUpdatedBy;
                    NewSecurityGroup.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
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

        public async Task CreateGroupSecurityObject(int SecurityGroupID, int[] SecurityAdminObjectList)
        {
            foreach (var AdminSecurityItem in SecurityAdminObjectList)
            {
                GroupSecuityObject groupSecurityObjects = new GroupSecuityObject()
                {
                    SecurityObjectId = AdminSecurityItem,
                    SecurityGroupId = SecurityGroupID,
                };
                await _context.AddAsync(groupSecurityObjects);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckMenuAccessAssociation(int SecurityGroupID, int CompanyID)
        {
            try
            {
                DBCommon DBC = new DBCommon(_context,_httpContextAccessor);
                bool sendemail = false;

                var pCompanyId = new SqlParameter("@CompanyId", CompanyID);
                var pSecurityGroupID = new SqlParameter("@SecurityGroupID", SecurityGroupID);

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
                    await SDE.SendMenuAccessAssociationsToAdmin(sb.ToString(), SecurityGroupID, CompanyID);
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
