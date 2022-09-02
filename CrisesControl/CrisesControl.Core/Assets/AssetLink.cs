using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Assets
{
    public class AssetLink
    {
        public string? IncidentName { get; set; }
        public string? TaskName { get; set; }
        public string? LinkType { get; set; }
        public int? IncidentID { get; set; }
        public int? TaskID { get; set; }
    }
}
