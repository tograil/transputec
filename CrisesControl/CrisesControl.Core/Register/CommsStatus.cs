using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Register
{
    public class CommsStatus
    {
        public string CurrentAction { get; set; }
        /// <summary>
        /// Call id stores the unique identifier of call 
        /// at providers end.
        /// </summary>
        public string CommsId { get; set; }
        /// <summary>
        /// Status is an arbitrary value set within the function.
        /// true/false
        /// </summary>
        public bool Status { get; set; }
    }
}
