namespace CrisesControl.Core.Models
{
    public partial class PriorityMethod
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int PriorityLevel { get; set; }
        public string Methods { get; set; } = null!;
        public string MessageType { get; set; } = null!;
    }
}
