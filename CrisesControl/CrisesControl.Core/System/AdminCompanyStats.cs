using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.System
{
    public class AdminCompanyStats
    {
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public string CompanyStatus { get; set; }
        public int ActiveStaff { get; set; }
        public int InActive { get; set; }
        public int Deleted { get; set; }
        public int PendingVerification { get; set; }
        public int StaffPendingVerification { get; set; }
        public int KeyHolderPendingVerification { get; set; }
        public int AdminPendingVerification { get; set; }
        public int TotalActiveAdmin { get; set; }
        public int TotalActiveKeyHolder { get; set; }
    }
}
