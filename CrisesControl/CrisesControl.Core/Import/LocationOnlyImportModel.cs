using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import
{
    public class LocationOnlyImportModel
    {
        public string SessionId { get; set; }

        public List<LocationData> LocationData { get; set; }
    }

    public class LocationData
    {
        public LocationData()
        {
            Action = "ADD";
            LocationStatus = 0;
        }
        public string LocationName { get; set; }
        public string LocationAddress { get; set; }
        public int LocationStatus { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}
