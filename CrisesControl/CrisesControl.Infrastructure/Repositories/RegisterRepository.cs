using CrisesControl.Core.Companies;
using CrisesControl.Core.Register.Repositories;
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
    public class RegisterRepository : IRegisterRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<RegisterRepository> _logger;
        public RegisterRepository(ILogger<RegisterRepository> logger, CrisesControlContext context)
        {
          this._logger = logger;
          this._context = context;
        }
        public async Task<bool> CheckCustomer(string CustomerId)
        {
            try
            {
                CustomerId = CustomerId.Trim().ToLower();
                var Customer = await _context.Set<Company>()
                                     .Include(c=>c.StdTimeZone)
                                     .Where(C => C.CustomerId == CustomerId)
                                     .AnyAsync();
                return Customer;
            }
            catch (Exception ex)
            {
               _logger.LogError("Error occured while seeding into database ");
                return false;
            }
        }
    }
}
