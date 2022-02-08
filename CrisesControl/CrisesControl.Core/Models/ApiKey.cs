namespace CrisesControl.Core.Models
{
    public partial class ApiKey
    {
        public int ApikeyId { get; set; }
        public string Apikey1 { get; set; } = null!;
        public string? Description { get; set; }
        public string? AllowedIp { get; set; }
        public int Status { get; set; }
    }
}
