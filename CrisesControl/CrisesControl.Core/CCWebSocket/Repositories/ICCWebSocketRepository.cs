using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CCWebSocket.Repositories
{
    public interface ICCWebSocketRepository
    {
        void RemoveFromClientList(int clientUserId, CCWebSocketHandler session);
        Task SendTo(int userId, string message);
        Task AddToClientList(int clientUserId, CCWebSocketHandler session);
        Task Echo(WebSocket webSocket);
    }
}
