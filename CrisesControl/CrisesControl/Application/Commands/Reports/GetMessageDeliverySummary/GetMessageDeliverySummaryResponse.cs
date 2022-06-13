using System.Net;

namespace CrisesControl.Api.Application.Commands.Reports.GetMessageDeliverySummary
{
    public class GetMessageDeliverySummaryResponse
    {
        public object Data { get; set; }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
