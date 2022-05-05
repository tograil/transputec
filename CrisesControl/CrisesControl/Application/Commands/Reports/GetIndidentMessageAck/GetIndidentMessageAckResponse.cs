using CrisesControl.Core.Reports;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck
{
    public class GetIndidentMessageAckResponse
    {
        public List<MessageAcknowledgements> Data { get; set; }
        public HttpStatusCode ErrorCode { get; set; }
    }
}
