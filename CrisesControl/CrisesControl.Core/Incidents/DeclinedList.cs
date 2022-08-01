using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class DeclinedList
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int TaskDeclinedBy { get; set; }
    }
}
