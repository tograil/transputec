using System;
using System.Collections.Generic;

namespace CC.Authority.Implementation.Models
{
    public partial class StdTimeZone
    {
        public int TimeZoneId { get; set; }
        public string? ZoneId { get; set; }
        public string? ZoneLabel { get; set; }
        public string? PortalTimeZone { get; set; }
    }
}
