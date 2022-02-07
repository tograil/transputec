namespace CrisesControl.Core.Models
{
    public partial class Icon
    {
        public int IconId { get; set; }
        public string IconTitle { get; set; } = null!;
        public string IconFile { get; set; } = null!;
        public string IconTags { get; set; } = null!;
        public int CompanyId { get; set; }
    }
}
