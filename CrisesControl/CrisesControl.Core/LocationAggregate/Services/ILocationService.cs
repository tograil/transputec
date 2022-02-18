using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationModel = CrisesControl.Core.Models.Location;

namespace CrisesControl.Core.LocationAggregate.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationModel>> GetAllLocations();
    }
}
