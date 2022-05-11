using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Reports;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageNoAck
{
    public class GetIndidentMessageNoAckResponse
    {
        public List<DataTablePaging> data { get; set; }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
