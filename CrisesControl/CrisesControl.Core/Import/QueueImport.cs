using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import
{
    public class QueueImport
    {
        public string MappingFileName { get; set; }
        public string DataFileName { get; set; }
        public string SessionId { get; set; }
        public bool SendInvite { get; set; }
        public string JobType { get; set; }
    }
}
