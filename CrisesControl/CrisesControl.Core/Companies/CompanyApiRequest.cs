using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Companies
{
    public class CompanyApiRequest
    {
        public int CompanyId { get; set; }
        public int ApiId { get; set; }
        public string CompanyName { get; set; }
        public string CustomerId { get; set; }
        public string InvitationCode { get; set; }
        public string ApiMode { get; set; }
        public string AppVersion { get; set; }
        public string ApiHost { get; set; }
        public string ApiVersion { get; set; }
        public string Platform { get; set; }

    }
}
