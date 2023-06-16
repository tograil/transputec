using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class UsrResponse
    {
        public int ActiveReponseID { get; set; }
        public string Response { get; set; }
        public string Comment { get; set; }
        public bool Done { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool MarkDone { get; set; }
    }
}
