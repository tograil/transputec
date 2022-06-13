using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Users
{
    public class UGroup
    {
        public string ObjectTableName { get; set; }
        public int SourceObjectPrimaryId { get; set; }
        public int ObjectMappingId { get; set; }
    }
}
