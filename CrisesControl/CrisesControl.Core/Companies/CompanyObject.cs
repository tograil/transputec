using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Companies
{
    public class CompanyObject
    {
        public int ObjectMappingId { get; set; }
        public int ObjectId { get; set; }
        public int? CompanyId { get; set; }
        public string? ObjectName { get; set; }
        public string? ObjectTableName { get; set; }
    }
}
