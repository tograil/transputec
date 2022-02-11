namespace CrisesControl.Core.Models
{
    public partial class UserModuleLink
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int UserId { get; set; }
        public decimal Xpos { get; set; }
        public decimal Ypos { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }
}
