using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Security
{
    public class CompanySecurityGroup
    {
        public int SecurityGroupId { get; set; }
     
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
       
    }
}
