namespace CrisesControl.Core.Models
{
    public partial class CompanyFtp
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string HostName { get; set; } = null!;
        public string? UserName { get; set; }
        public string? SecurityKey { get; set; }
        public string? Protocol { get; set; }
        public int? Port { get; set; }
        public string? RemotePath { get; set; }
        public string? LogonType { get; set; }
        public bool DeleteSourceFile { get; set; }
        public string? ShafingerPrint { get; set; }
    }
}
