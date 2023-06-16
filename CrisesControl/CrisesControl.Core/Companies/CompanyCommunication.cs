using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Companies
{
    public class CompanyCommunication
    {
      
        public List<PriorityInterval> Priority { get; set; }
        
        public List<PriorityMethod> PriorityMethod { get; set; }
        public List<CommsObjects> ObjectInfo { get; set; }
       
        [NotMapped]
        public object BillUsers { get; set; }
        public string HasLowBalance { get; set; }
    }

    public class CommsObjects {
        public int MethodId { get; set; }
        public bool ServiceStatus { get; set; }
        public int Status { get; set; }
        public string MethodName { get; set; }
    }
}
