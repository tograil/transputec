namespace CrisesControl.Api.Application.Commands.CustomEventLog.ExportEventLog
{
    public class ExportEventLogResponse
    {
        public string FileName { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
