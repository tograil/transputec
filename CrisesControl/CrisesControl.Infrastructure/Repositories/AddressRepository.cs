using CrisesControl.Core.AddressDetails.Repositories;
using CrisesControl.Core.Models;
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
    public class AddressRepository : IAddressRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<AddressRepository> _logger;

        public AddressRepository(ILogger<AddressRepository> logger, CrisesControlContext context)
        {
            this._context = context;
            this._logger = logger;
        }
        public async Task<int> AddAddress(Address address)
        {
            await _context.AddAsync(address);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Address Added "+ address.AddressId);
            return address.AddressId;
        }

        public async Task<bool> DeleteAddress(int AddressId)
        {
            var add = await this.GetAddress(AddressId);
            if (add !=null)
            {
                _context.Remove(add);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
           
        }

        public async Task<Address> GetAddress(int AddressId)
        {
            var address= await _context.Set<Address>().FirstOrDefaultAsync(x => x.AddressId == AddressId);
            if (address!=null)
            {
                return address;
            }
            return null;
        }

        public async Task<List<Address>> GetAllAddress(int PageNumber, int PageSize, string OrderBy)
        {
            return await _context.Set<Address>().Take(PageSize).Skip(PageNumber).ToListAsync();
        }

        public async Task<int> UpdateAddress(Address address)
        {
             _context.Update(address);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Address Updated " + address.AddressId);
            return address.AddressId;
        }
    }
}
