namespace CrisesControl.Core.Models
{
    public partial class PriorityInterval
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int? CascadingPlanId { get; set; }
        public string MessageType { get; set; } = null!;
        public int Priority { get; set; }
        public int Interval { get; set; }
        public string? Methods { get; set; }
    }
}
