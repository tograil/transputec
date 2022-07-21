using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using CrisesControl.Core.Security;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
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
        //private readonly SendEmail _SDE;
        // private readonly DBCommon _DBC;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SecurityRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
           // this._SDE = SDE;
           // this._DBC = DBC;
        }

        public async Task<int> AddSecurityGroup(SecurityGroup securityGroup)
        {
            await _context.AddAsync(securityGroup);
            await _context.SaveChangesAsync();
            return securityGroup.SecurityGroupId;
        }

        public async Task<SecurityGroup> GetSecurityGroup(int SecurityGroupId, int CompanyID)
        {
            try { 
            var GroupData = await _context.Set<SecurityGroup>()
                             .Where(GRP=> GRP.CompanyId == CompanyID && GRP.SecurityGroupId == SecurityGroupId
                             ).FirstOrDefaultAsync();
            return GroupData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
 /*
-- Template generated from Template Explorer using:
-- Create Procedure(New Menu).SQL
--
-- Use the Specify Values for Template Parameters
-- command(Ctrl-Shift-M) to fill in the parameter
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Confidence Selomo>
-- Create date: <12/07/2022,,>
-- Description:	<Linq to store procedure to get all security objects>
-- =============================================
CREATE PROCEDURE Pro_Get_AllSecurityObjects
	-- Add the parameters for the stored procedure here

    @CompanyID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

    SET NOCOUNT ON;

    -- Insert statements for procedure here

    select
       SO.SecurityObjectID,
       SO.ParentID,
       SO.TypeID,
       ObjectType = ST.Code,
       SO.SecurityKey,
       SO.Name,
       SO.Status,
       SO.Target,
       SO.RequireKeyHolder,
       SO.RoleID,
       SO.RequireAdmin,
       SO.MenuOrder,
       SO.ForIncidentManager
FROM
 SecurityObjects as SO
 INNER join SecurityObjectType as ST on SO.TypeId = ST.SecurityObjectTypeId
 INNER join CompanyPackageFeature as PF on SO.SecurityObjectId = PF.SecurityObjectId
 INNER join Company as C on PF.CompanyId = C.CompanyId
 where C.CompanyId = @CompanyID
 AND (SO.TypeId = 7 OR SO.TypeId = 8)
 order by SO.MenuOrder
END
GO
 */

        public async Task<List<SecurityAllObjects>> GetAllSecurityObjects(int CompanyID)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
               var allObjects= await _context.Set<SecurityAllObjects>().FromSqlRaw("EXEC Pro_Get_AllSecurityObjects @CompanyID", pCompanyID).ToListAsync();
                return allObjects;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        //public object GetSecurityGroup(int CompanyID, int SecurityGroupId)
        //{
           
        //    try
        //    {
        //        var SecurityInfo = (from SG in _context.Set<SecurityGroup>()
        //                            join UC in _context.Set<User>() on SG.CreatedBy equals UC.UserId
        //                            join UU in _context.Set<User>() on SG.UpdatedBy equals UU.UserId
        //                            where SG.CompanyId == CompanyID && SG.SecurityGroupId == SecurityGroupId
        //                            select new //anonymous method
        //                            {
        //                                SecurityGroupId = SG.SecurityGroupId,
        //                                CompanyId = SG.CompanyId,
        //                                Name = SG.Name,
        //                                Description = SG.Description,
        //                                SecurityObjects = (from SO in _context.Set<SecurityObject>()
        //                                                   join GSO in _context.Set<GroupSecuityObject>() on SO.SecurityObjectId equals GSO.SecurityObjectId
        //                                                   join PF in _context.Set<CompanyPackageFeature>() on SO.SecurityObjectId equals PF.SecurityObjectId
        //                                                   where GSO.SecurityGroupId == SecurityGroupId && PF.CompanyId == CompanyID
        //                                                   select new
        //                                                   {
        //                                                       SecurityObjectID = SO.SecurityObjectId,
        //                                                       ParentID = SO.ParentId,
        //                                                       TypeID = SO.TypeId,
        //                                                       SecurityKey = SO.SecurityKey,
        //                                                       Name = SO.Name,
        //                                                       Status = SO.Status,
        //                                                   }),
        //                                SG.UserRole,
        //                                Status = SG.Status,
        //                                CreatedByName = new UserFullName { Firstname = UC.FirstName, Lastname = UC.LastName },
        //                                UpdatedByName = new UserFullName { Firstname = UU.FirstName, Lastname = UU.LastName },
        //                                SG.CreatedOn,
        //                                SG.UpdatedOn
        //                            }).FirstOrDefault();

                
        //            return SecurityInfo;
                
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<int> UpdateSecurityGroup(SecurityGroup securityGroup)
        {
             _context.Update(securityGroup);
            await _context.SaveChangesAsync();
            return securityGroup.SecurityGroupId;
        }
        public async Task<bool> CheckMenuAccessAssociation(int SecurityGroupID, int CompanyID)
        {
            try
            {
                bool sendemail = false;
                DBCommon _DBC = new DBCommon(_context, _httpContextAccessor);
                SendEmail _SDE = new SendEmail(_context, _DBC);
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
                        sb.AppendLine("<tr><td>" + item.ModuleItem + "</td><td>" + _DBC.UserName(item.UserName) + "</td></tr>");
                    }
                    sb.AppendLine("</table>");
                  
                   await _SDE.SendMenuAccessAssociationsToAdmin(sb.ToString(), SecurityGroupID, CompanyID);
                }

                return sendemail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public async Task<List<GroupSecuityObject>> GetGroupSecuityObject(int SecurityGroupId)
        {
          var GRPITMList=  await _context.Set<GroupSecuityObject>()
             .Where(GRPITM=>GRPITM.SecurityGroupId == SecurityGroupId).ToListAsync();
            return GRPITMList;
        }

        public async Task<int> AddGroupSecuityObject(GroupSecuityObject GroupSecurityObject)
        {
            await _context.AddAsync(GroupSecurityObject);
            await _context.SaveChangesAsync();
            return GroupSecurityObject.GroupSecurityObjectId;
        }
        public async Task<int> UpdateGroupSecuityObject(GroupSecuityObject GroupSecurityObject)
        {
             _context.Update(GroupSecurityObject);
            await _context.SaveChangesAsync();
            return GroupSecurityObject.GroupSecurityObjectId;
        }
        public async Task DeleteGroupSecuityObject(GroupSecuityObject GroupSecurityObject)
        {
            _context.Remove(GroupSecurityObject);
            await _context.SaveChangesAsync();
          
        }

        public async Task DeleteUserSecurityGroup(List<UserSecurityGroup> userSecurityGroup)
        {
            _context.RemoveRange(userSecurityGroup);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserSecurityGroup>> GetUserSecurityGroup(int SecurityGroupId)
        {
            var delUserSecurityGroup = await  _context.Set<UserSecurityGroup>().Where(USG=> USG.SecurityGroupId == SecurityGroupId ).ToListAsync();
            return delUserSecurityGroup;
        }
    }
}
