using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Reports;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentMessageAck
{
    public class GetIncidentMessageAckResponse
    {
        public DataTablePaging Data { get; set; }
        public HttpStatusCode ErrorCode { get; set; }
    }
}
