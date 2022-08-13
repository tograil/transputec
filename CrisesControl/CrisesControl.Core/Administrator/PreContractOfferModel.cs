using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class PreContractOfferModel
    {
        public decimal MonthlyContractValue { get; set; }
        public decimal YearlyContractValue { get; set; }
        public decimal KeyHolderRate { get; set; }
        public decimal StaffRate { get; set; }
        public int KeyHolderLimit { get; set; }
        public int StaffLimit { get; set; }
    }
}
