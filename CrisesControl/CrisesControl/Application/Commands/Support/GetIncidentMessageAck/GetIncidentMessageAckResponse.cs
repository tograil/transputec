using CrisesControl.Core.Compatibility;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Support.GetIncidentMessageAck
{
    public class GetIncidentMessageAckResponse
    {
        public DataTablePaging Data { get; set; }
        public HttpStatusCode ErrorCode { get; set; }
    }
}
