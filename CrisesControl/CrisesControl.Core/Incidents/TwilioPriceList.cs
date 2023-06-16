using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class TwilioPriceList
    {
        public string? ISDCode { get; set; }
        public string? ChannelType { get; set; }
        public decimal BasePrice { get; set; } 
        public decimal CurrentPrice { get; set; }
    }
}
