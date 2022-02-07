namespace CrisesControl.Core.Models
{
    public partial class MethodsToLog
    {
        public int Id { get; set; }
        public string ControllerName { get; set; } = null!;
        public string MethodName { get; set; } = null!;
        public int Status { get; set; }
    }
}
