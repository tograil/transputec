using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class ConferenceUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserPhoto { get; set; }
        public string Isdcode { get; set; }
        public string MobileNo { get; set; }
        public string PrimaryEmail { get; set; }
        public int UserId { get; set; }
    }
}
