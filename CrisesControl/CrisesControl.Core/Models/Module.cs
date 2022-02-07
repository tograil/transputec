namespace CrisesControl.Core.Models
{
    public partial class Module
    {
        public int ModuleId { get; set; }
        public string? ProductCode { get; set; }
        public string ModuleName { get; set; } = null!;
        public int SecurityObjectId { get; set; }
        public int TransactionTypeId { get; set; }
        public int Status { get; set; }
        public string ModuleType { get; set; } = null!;
        public string? ParameterKey { get; set; }
        public string? ModuleChargeType { get; set; }
        public double? ModuleRate { get; set; }
        public int? ModuleOrder { get; set; }
        public int? LinkId { get; set; }
        public int? ParentId { get; set; }
    }
}
