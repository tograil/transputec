using MediatR;
using System.Net.WebSockets;

namespace CrisesControl.Api.Application.Commands.CCWebSocket.Get
{
    public class GetRequest:IRequest<bool>
    {
        public WebSocket WebSocket { get; set; }
    }
}
