using CrisesControl.Core.LocationAggregate.Services;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public class LocationService: ILocationService
    {
        private readonly CrisesControlContext _context;

        public LocationService(CrisesControlContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Location>> GetAllLocations()
        {
            return await _context.Set<Location>().AsNoTracking().ToArrayAsync();
        }

    }
}
