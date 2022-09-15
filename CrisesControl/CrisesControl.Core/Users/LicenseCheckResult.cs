using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Users
{
    public class LicenseCheckResult
    {
        public int ExceedingStaff { get; set; }
        public int ExceedingKeyholder { get; set; }
        public int StaffLimit { get; set; }
        public int KeyholderLimit { get; set; }
        public double PerStaffCost { get; set; }
        public double PerKeyholderCost { get; set; }
        public double StaffCost { get; set; }
        public double KeyholderCost { get; set; }
        public int ExtraKeyholders { get; set; }
        public int ExtraStaff { get; set; }
        public bool ConfirmAction { get; set; }
        public string Duration { get; set; }
        public string CompanyProfile { get; set; }
        public bool OnTrial { get; set; }


    }
}
