using CrisesControl.Core.Incidents;
using CrisesControl.Core.Messages;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Messaging.GetPingInfo
{
    public class GetPingInfoResponse
    {
        public PingInfoReturn Data { get; set; }
        public HttpStatusCode ErrorCode { get; set; }
    }
}
