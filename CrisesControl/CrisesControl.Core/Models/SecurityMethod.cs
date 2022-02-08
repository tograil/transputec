namespace CrisesControl.Core.Models
{
    public partial class SecurityMethod
    {
        public int MethodId { get; set; }
        public string? MethodName { get; set; }
        public string? SecurityKey { get; set; }
        public string? Target { get; set; }
    }
}
