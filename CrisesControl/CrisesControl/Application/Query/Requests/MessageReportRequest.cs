using CrisesControl.Core.Compatibility;

namespace CrisesControl.Api.Application.Query.Requests
{
    public class MessageReportRequest: DataTableAjaxPostModel
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string MessageType { get; set; }
        public int DrillOpt { get; set; }
        public int GroupId { get; set; }
        public int ObjectMappingId { get; set; }
        public string CompanyKey { get; set; }
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }
    }
}
