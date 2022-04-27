using AutoMapper;
using CrisesControl.Core.Communication;
using CrisesControl.Core.Communication.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
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

namespace CrisesControl.Infrastructure.Repositories {
    public class CommunicationRepository : ICommunicationRepository {
        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private int UserID;
        private int CompanyID;

        public CommunicationRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<UserConferenceItem>> GetUserActiveConferences() {
            try {

                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
                CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));

                var pUserId = new SqlParameter("@UserID", UserID);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);

                var result = await _context.Set<UserConferenceItem>().FromSqlRaw("exec Pro_Get_User_Active_Conference_List {0},{1}", pCompanyID, pUserId).ToListAsync();

                return result;
            } catch (Exception) {

                throw;
            }
        }

    }
}
