using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Users
{
    public class PasswordChangeModel
    {
        public string Action { get; set; }
        public int UserID { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string OTPCode { get; set; }
        public string Return { get; set; }
        public string OTPMessage { get; set; }
        public string Source { get; set; }
        public string Method { get; set; }
    }
}
