using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CompanyParameters
{
    public class CommsMethodPriority
    {
        public int CascadingPlanID { get; set; }
        public string MessageType { get; set; }
        public int Priority { get; set; }
        public int Interval { get; set; }
        public string MethodString { get; set; }
        public bool ServiceStatus { get; set; }
        public int Status { get; set; }
        public int MethodId { get; set; }
        public string MethodName { get; set; }
        [NotMapped]
        public int[] Methods { get; set; }


    }
}
