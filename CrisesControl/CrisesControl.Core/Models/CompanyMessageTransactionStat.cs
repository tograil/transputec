namespace CrisesControl.Core.Models
{
    public partial class CompanyMessageTransactionStat
    {
        public int CompanyId { get; set; }
        public long? TotalPush { get; set; }
        public long? TotalPhone { get; set; }
        public long? TotalText { get; set; }
        public long? TotalEmail { get; set; }
    }
}
