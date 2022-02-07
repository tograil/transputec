namespace CrisesControl.Core.Models
{
    public partial class ModuleLink
    {
        public int LinkId { get; set; }
        public int ModuleObjectId { get; set; }
        public int ItemObjectId { get; set; }
        public int TransactionTypeId { get; set; }
        public int Status { get; set; }
    }
}
