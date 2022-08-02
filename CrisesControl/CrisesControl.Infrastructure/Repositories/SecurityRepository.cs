using CrisesControl.Core.Security;
using CrisesControl.Infrastructure.Context;
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
        private readonly SendEmail _SDE;
        private readonly DBCommon _DBC;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SecurityRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor)
        public SecurityRepository(CrisesControlContext context)
        {
            this._context = context;    
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
    }
}
