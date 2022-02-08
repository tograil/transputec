namespace CrisesControl.Core.Models
{
    public partial class DashboardModule
    {
        public int ModuleId { get; set; }
        public string ModulePage { get; set; } = null!;
        public string ModuleName { get; set; } = null!;
        public string ContainerId { get; set; } = null!;
        public decimal Xpos { get; set; }
        public decimal Ypos { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal MinWidth { get; set; }
        public decimal MinHeight { get; set; }
        public decimal MaxWidth { get; set; }
        public decimal MaxHeight { get; set; }
        public string? ResizeHandle { get; set; }
        public string? ColorScheme { get; set; }
        public int AllowResize { get; set; }
        public int AllowMove { get; set; }
        public int Locked { get; set; }
        public int Status { get; set; }
        public string WidgetFilePath { get; set; } = null!;
    }
}
