using CrisesControl.Core.Administrator;
using CrisesControl.Core.Administrator.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class AdminRepository: IAdminRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<AdminRepository> _logger;
        public AdminRepository(CrisesControlContext context, ILogger<AdminRepository> logger)
        {
            this._context=context;
            this._logger=logger;
        }
        public async Task<List<LibIncident>> GetAllLibIncident()
        {
            try
            {
                var allLibIncidents = await _context.Set<LibIncident>("exec Pro_Admin_GetAllLibIncidents").ToListAsync();

                if (allLibIncidents != null)
                {
                    return allLibIncidents;
                }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new List<LibIncident>();
        }
    }
}
