using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Models
{
    public class ValidateEmailReponseModel
    {
        public string SSOType { get; set; }
        public string SSOEnabled { get; set; }
        public string SSOIssuer { get; set; }
        public string SSOSecret { get; set; }
    }
}
