using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import
{
    public class ProcessImport
    {
        public string SessionId { get; set; }
        public bool SendInvite { get; set; }
        public int CreatedBy { get; set; }
    }
}
