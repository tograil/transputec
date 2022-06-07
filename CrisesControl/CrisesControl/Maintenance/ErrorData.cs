namespace CrisesControl.Api.Maintenance
{
    public class ErrorData
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public int RegID { get; set; }
    }
}