namespace CrisesControl.Core.Models
{
    public partial class SegGroupDepartmentLink
    {
        public int GroupDepartmentId { get; set; }
        public int DepartmentId { get; set; }
        public int GroupId { get; set; }
        public int CompanyId { get; set; }
    }
}
