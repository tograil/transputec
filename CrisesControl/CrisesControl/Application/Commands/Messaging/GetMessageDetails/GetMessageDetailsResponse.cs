using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Messages;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageDetails
{
    public class GetMessageDetailsResponse
    {
        public object Data { get; set; }
        public HttpStatusCode ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
