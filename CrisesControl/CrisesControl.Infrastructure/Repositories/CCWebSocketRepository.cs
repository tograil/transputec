using CrisesControl.Core.CCWebSocket;
using CrisesControl.Core.CCWebSocket.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using CrisesControl.Infrastructure.Services.Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CrisesControl.Infrastructure.Repositories
{
    public class CCWebSocketRepository:ICCWebSocketRepository
    {
        private readonly ILogger<CCWebSocketRepository> _logger;
        private readonly ConcurrentDictionary<string, WebSocket> _sockets;
        public CCWebSocketRepository( ILogger<CCWebSocketRepository> logger)
        {
            _sockets = new ConcurrentDictionary<string, WebSocket>();
            _logger = logger;
        }
             
        public async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            _logger.Log(LogLevel.Information, "Message received from Client");

            while (!result.CloseStatus.HasValue)
            {
                var serverMsg = Encoding.UTF8.GetBytes($"Server: Hello. You said: {Encoding.UTF8.GetString(buffer)}");
                await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                _logger.Log(LogLevel.Information, "Message sent to Client");

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                _logger.Log(LogLevel.Information, "Message received from Client");

            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            _logger.Log(LogLevel.Information, "WebSocket connection closed");
        }
        public async Task ProcessWebsocketSession(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result =
                await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string clientId = Encoding.UTF8.GetString(buffer, 0, result.Count);

            // record the client id and it's websocket instance  
            if (_sockets.TryGetValue(clientId, out var wsi))
            {
                if (wsi.State == WebSocketState.Open)
                {
                    Console.WriteLine($"abort the before clientId named {clientId}");
                    await wsi.CloseAsync(WebSocketCloseStatus.InternalServerError,
                        "A new client with same id was connected!",
                        CancellationToken.None);
                }

                _sockets.AddOrUpdate(clientId, webSocket, (x, y) => webSocket);
            }
            else
            {
                Console.WriteLine($"add or update {clientId}");
                _sockets.AddOrUpdate(clientId, webSocket, (x, y) => webSocket);
            }

            while (!result.CloseStatus.HasValue)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            Console.WriteLine("close=" + clientId);

            _sockets.TryRemove(clientId, out _);
        }


    }
}
