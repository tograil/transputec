using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import
{
    public class ImportResult
    {
        public int TotalImport { get; set; }
        public int TotalUpdate { get; set; }
        public int TotalSkip { get; set; }
        public int TotalDelete { get; set; }
        public string ResultFile { get; set; }
    }
}
