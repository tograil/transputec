using System;

namespace CrisesControl.Core.Models
{
    public partial class PreContractOffer
    {
        public int OfferId { get; set; }
        public int CompanyId { get; set; }
        public decimal MonthlyContractValue { get; set; }
        public decimal YearlyContractValue { get; set; }
        public decimal KeyHolderRate { get; set; }
        public int KeyHolderLimit { get; set; }
        public decimal StaffRate { get; set; }
        public int StaffLimit { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
