namespace CrisesControl.Api.Application.Commands.Reports.GetMessageDeliveryReport
{
    public class GetMessageDeliveryReportRouteRequest
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}
