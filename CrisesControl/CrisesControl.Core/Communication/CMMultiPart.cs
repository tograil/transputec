using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Communication
{
    public class CMMultiPart
    {
        public string Identifier { get; set; }
        public int Parts { get; set; }
        public int Part { get; set; }
        public decimal Totalprice { get; set; }
        public decimal Localprice { get; set; }
    }
}
