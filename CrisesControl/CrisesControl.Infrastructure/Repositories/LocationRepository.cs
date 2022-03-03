using CrisesControl.Core.LocationAggregate.Services;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Task<Core.LocationAggregate.Location> CreateLocation(Core.LocationAggregate.Location location)
        {
            throw new NotImplementedException();
        }

        public Task DeleteLocation(int locationId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Location>> GetAllLocations()
        {
            return await _context.Set<Location>().AsNoTracking().ToArrayAsync();
        }

        public Task<Core.LocationAggregate.Location> GetLocationById(int locationId)
        {
            throw new NotImplementedException();
        }

        public Task<Core.LocationAggregate.Location> UpdateLocation(Core.LocationAggregate.Location location)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Core.LocationAggregate.Location>> ILocationRepository.GetAllLocations()
        {
            throw new NotImplementedException();
        }
    }
}
