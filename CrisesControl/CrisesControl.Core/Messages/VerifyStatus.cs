using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class VerifyStatus
    {
        /// <summary>
        /// Call id stores the unique identifier of call 
        /// at providers end.
        /// </summary>
        public string Sid { get; set; }
        /// <summary>
        /// Status is an arbitrary value set within the function.
        /// true/false
        /// </summary>
        public string Status { get; set; }
    }
}
