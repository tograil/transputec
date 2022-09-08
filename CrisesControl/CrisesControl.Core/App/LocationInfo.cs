using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.App
{
    public class LocationInfo
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTimeOffset UserDeviceTime { get; set; }
    }
}
