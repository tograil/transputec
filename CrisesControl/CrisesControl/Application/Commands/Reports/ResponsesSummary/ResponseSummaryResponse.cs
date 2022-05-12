using CrisesControl.Core.Reports;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Reports.ResponsesSummary
{
    public class ResponseSummaryResponse
    {
        public List<ResponseSummary> Data { get; set; }
        public HttpStatusCode ErrorCode { get; set; }
    }
}
