
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CCWebSocket.Repositories
{
    public interface ICCWebSocketRepository
    {
        
        Task Echo(WebSocket webSocket);
        Task ProcessWebsocketSession(HttpContext context, WebSocket webSocket);
    }
}
