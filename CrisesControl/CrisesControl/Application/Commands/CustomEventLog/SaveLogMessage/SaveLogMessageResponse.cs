namespace CrisesControl.Api.Application.Commands.CustomEventLog.SaveLogMessage
{
    public class SaveLogMessageResponse
    {
        public string Result { get; set; }
        public int UserID { get; set; }
        public int LocationID { get; set; }
        public int GroupID { get; set; }
        public int UserSecurityGroupID { get; set; }
        public int Success { get; set; }
        public int ResultID { get; set; }
    }
}
