using MediatR;
using System.Net.WebSockets;

namespace CrisesControl.Api.Application.Commands.CCWebSocket.ProcessWebsocketSession
{
    public class ProcessWebsocketSessionRequest:IRequest<bool>
    {
        public WebSocket WebSocket { get; set; }
        public HttpContext Context { get; set; }
    }
}
