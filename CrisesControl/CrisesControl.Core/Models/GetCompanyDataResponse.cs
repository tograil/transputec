using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Models
{
    public class GetCompanyDataResponse
    {
        public string CompanyProfile { get; set; }
        public bool OnTrial { get; set; }
        public int TotalLocation { get; set; }
        public int TotalGroup { get; set; }
        public int TotalKeyHolder { get; set; }
        public int TotalActiveKeyHolder { get; set; }
        public int TotalUsers { get; set; }
        public int TotalActiveUsers { get; set; }
        public int TotalMessage { get; set; }
        public int TotalIncident { get; set; }
        public int ActiveIncident { get; set; }
        public int TotalWithKC { get; set; }
    }
}
