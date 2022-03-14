using CrisesControl.Core.LocationAggregate;
using CrisesControl.Core.LocationAggregate.Services;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class LocationRepository: ILocationRepository
    {
        private readonly CrisesControlContext _context;

        public LocationRepository(CrisesControlContext context)
        {
            _context = context;
        }

        public async Task<int> CreateLocation(Location location, CancellationToken token)
        {
            await _context.AddAsync(location, token);
            await _context.SaveChangesAsync(token);
            return location.LocationId;
        }

        public async Task<int> DeleteLocation(int locationId, CancellationToken token)
        {
            await _context.AddAsync(locationId);

            await _context.SaveChangesAsync(token);

            return locationId;
        }

        public async Task<IEnumerable<Location>> GetAllLocations(int companyId)
        {
            return await _context.Set<Location>().AsNoTracking().Where(t=>t.CompanyId == companyId).ToListAsync();
        }

        public async Task<Location> GetLocationById(int locationId)
        {
            return await _context.Set<Location>().AsNoTracking().Where(t => t.LocationId == locationId).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateLocation(Location location, CancellationToken token)
        {
            await _context.AddAsync(location, token);
            await _context.SaveChangesAsync(token);
            return location.LocationId;
        }

        public bool CheckDublicate(Location location)
        {
           return _context.Set<Location>().Where(t=>t.LocationName.Equals(location.LocationName)).FirstOrDefault() != null;
        }
    }
}
