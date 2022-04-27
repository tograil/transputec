using CrisesControl.Core.Communication;
using CrisesControl.Core.Communication.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
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
    public class CommunicationRepository : ICommunicationRepository
    {
        private readonly CrisesControlContext _context;
        
        public CommunicationRepository(CrisesControlContext context)
        {
            this._context=context;
            

        }
        public async Task<IEnumerable<ConferenceDetails>> GetUserActiveConferenceList(int UserID, int CompanyID)
        {
            try
            {
                var pUserId = new SqlParameter("@UserID", UserID);
                var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
                return await _context.Set<ConferenceDetails>().FromSqlRaw("EXEC Pro_Get_User_Active_Conference_List @UserID,@CompanyID", pUserId, pCompanyId).ToListAsync();
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
