namespace CrisesControl.Core.Models
{
    public partial class OrderDetail
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int? ModuleId { get; set; }
        public decimal? Rate { get; set; }
        public int? Unit { get; set; }
        public decimal? Amount { get; set; }
        public string? Added { get; set; }
        public decimal? Discount { get; set; }
    }
}
