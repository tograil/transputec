using CrisesControl.Core.ExTriggers;
using CrisesControl.Core.ExTriggers.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace CrisesControl.Infrastructure.Repositories
{
    public class ExTriggerRepository : IExTriggerRepository
    {
        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private int UserID;
        private int CompanyID;
        public ExTriggerRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<ExTriggerList>> GetAllExTrigger()
        {
            try
            {
                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
                CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
                var pUserId = new SqlParameter("@UserID", UserID);
                var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
                var result = _context.Set<ExTriggerList>().FromSqlRaw("EXEC Pro_Get_Ex_Trigger @CompanyID,@UserID", pCompanyId, pUserId);

                var resultlist = await result.ToListAsync();
                if (resultlist != null)
                {
                    return resultlist;
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            return new List<ExTriggerList>();
        }
        /*Create a new store procedure
         * 
         * ALTER PROCEDURE [dbo].[Pro_Get_1_Ex_Trigger]
         *  @ExTriggerID INT, @CompanyID INT
         *   AS
         *   BEGIN
         *       SET NOCOUNT ON;
         *       SELECT   DISTINCT [ExTriggerID],
         *                         EX.[CompanyId],
         *                         [JobName],
         *                         [JobDescription],
         *                         [CommandLine],
         *                         [CommandLineParams],
         *                         [ActionType],
         *                         [IsEnabled],
         *                         [FailureReportEmail],
         *                         [SourceEmail],
         *                         [JobKey],
         *                         [CreatedDate],
         *                         EX.[CreatedBy],
         *                         [UpdatedDate],
         *                         EX.[UpdatedBy],
         *                         [SourceNumber],
         *                         [SourceNumberISD],
         *                         [ColumnMappingFilePath],
         *                         [ColumnMappingFileType],
         *                         [ImportFileType],
         *                         [FileHasHeader],
         *                         [Delimiter],
         *                         [SendInvite],
         *                         [SMSKey],
         *                         [AutoForceVerify]
         *       FROM     [dbo].[ExTrigger] AS EX WITH (NOLOCK)
         *                INNER JOIN
         *                [dbo].[Company] AS C WITH (NOLOCK)
         *                ON c.CompanyId = ex.CompanyId
         *       WHERE    EX.CompanyId = @CompanyID
         *                AND [ExTriggerID] = @ExTriggerID
         *                AND EX.UpdatedBy IN (SELECT UserId
         *                                     FROM   dbo.Get_Segregated_UserList(@CompanyID, c.UpdatedBy))
         *                AND EX.ActionType IN ('Incident', 'Ping', 'LOCATION', 'GROUP', 'DEPARTMENT', 'USER')
         *       ORDER BY [ExTriggerID] DESC;
         *       SET NOCOUNT OFF;
         *   END
         *
         * 
         * 
         * 
         */


        public async Task<IEnumerable<ExTriggerList>> GetExTrigger(int ExTriggerID, int CompanyID)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pExTriggerID = new SqlParameter("@ExTriggerID", ExTriggerID);
                var result = await _context.Set<ExTriggerList>().FromSqlRaw("EXEC Pro_Get_1_Ex_Trigger @ExTriggerID, @CompanyID", pCompanyID, pExTriggerID).ToListAsync();
                if (result != null)
                {
                    return result;
                }
                else
                {
                    return new List<ExTriggerList>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<IEnumerable<ExTriggerList>> GetImpTrigger()
        {
            try
            {
                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
                CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));

                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pUserID = new SqlParameter("@UserID", UserID);
                var result = await _context.Set<ExTriggerList>().FromSqlRaw("EXEC Pro_Get_Import_Trigger @CompanyID, @UserID", pCompanyID, pUserID).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return new List<ExTriggerList>();
        }
    }
}
