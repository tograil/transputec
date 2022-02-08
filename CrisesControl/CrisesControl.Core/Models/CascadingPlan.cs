namespace CrisesControl.Core.Models
{
    public partial class CascadingPlan
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; } = null!;
        public string PlanType { get; set; } = null!;
        public int CompanyId { get; set; }
        public bool LaunchSos { get; set; }
        public int LaunchSosinterval { get; set; }
    }
}
