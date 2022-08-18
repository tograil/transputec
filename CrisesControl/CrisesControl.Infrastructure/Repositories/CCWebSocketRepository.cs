using CrisesControl.Core.CCWebSocket;
using CrisesControl.Core.CCWebSocket.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services.Jobs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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
        public  int UserId = 0;
        public  List<SocketClientList> wsClientList = new List<SocketClientList>();
        static readonly string Icon = "assets/images/messages-icon.png";
        private readonly CrisesControlContext _context;
        private readonly ILogger<CCWebSocketRepository> _logger;
        public CCWebSocketRepository(CrisesControlContext context, ILogger<CCWebSocketRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task SetClientUserId(CCWebSocketHandler session)
        {
            string sesPath = session.WebSocketContext.QueryString["UserId"];

            //string[] pathParts = sesPath.Trim('/').Split('/');
            //if (pathParts.Length >= 2) {
            //if (pathParts[0] == "portalconnect") {
            int.TryParse(sesPath, out UserId);
            //}
            //}
        }

        public async Task AddToClientList(int clientUserId, CCWebSocketHandler session)
        {
            var userClient = wsClientList.Where(s => s.UserId == clientUserId).Any();
            //if (!userClient) {
            wsClientList.Add(new SocketClientList { UserId = clientUserId, CCWebSocketHandle = session });
            //}
        }

        public async  Task SendTo(int userId, string message)
        {
            var userClient = wsClientList.Where(s => s.UserId == userId).ToList();
            foreach (var client in userClient)
            {
                if (client != null)
                {
                   await client.CCWebSocketHandle.SendAsync(message,userClient);
                }
            }
        }

        public  void RemoveFromClientList(int clientUserId, CCWebSocketHandler session)
        {
            var userClient = wsClientList.Where(s => s.UserId == clientUserId).ToList();
            foreach (var client in userClient)
            {
                if (client.CCWebSocketHandle.WebSocketContext.SecWebSocketKey != null)
                {
                    if (client.CCWebSocketHandle.WebSocketContext.SecWebSocketKey == session.WebSocketContext.SecWebSocketKey)
                    {
                        wsClientList.Remove(client);
                    }
                }
            }
        }

        public  void SendCountToUsersByMessage(int tblMessageId)
        {
            try
            {

               

                var ptblMessageId = new SqlParameter("@MessageId", tblMessageId);
                var result = _context.Set<MessageCountResponse>().FromSqlRaw("Pro_Get_Message_Count_By_Message @MessageId", ptblMessageId)
                    .Where(w => wsClientList.Select(u => u.UserId).Contains(w.UserId)).ToList();

                if (result.Count > 0)
                {
                    foreach (var msgCount in result)
                    {
                        int totalCount = msgCount.PingCount + msgCount.IncidentCount;
                        string message = "{\"Type\":\"total_count\",\"MessageCount\":" + totalCount + ",\"TaskCount\":" + msgCount.TaskCount + ",\"Icon\":\"" + Icon + "\",\"Description\":\"" + HttpUtility.JavaScriptStringEncode(msgCount.MessageText) + "\",\"MessageType\":\"" + msgCount.MessageType + "\"}";
                        SendTo(msgCount.UserId, message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public  void SendMessageCountToUsersByMessage(int tblMessageId)
        {

            try
            {
             

                var ptblMessageId = new SqlParameter("@MessageId", tblMessageId);
                var result =_context.Set<MessageCountResponse>().FromSqlRaw("exec Pro_Get_Message_Count_By_Message @MessageId", ptblMessageId)
                    .Where(w => wsClientList.Select(u => u.UserId).Contains(w.UserId)).ToList();

                if (result.Count > 0)
                {
                    foreach (var msgCount in result)
                    {
                        int totalCount = msgCount.PingCount + msgCount.IncidentCount;
                        string message = "{\"Type\":\"message_count\",\"MessageCount\":" + totalCount + ",\"Icon\":\"" + Icon + "\",\"Description\":\"" + HttpUtility.JavaScriptStringEncode(msgCount.MessageText) + "\",\"MessageType\":\"" + msgCount.MessageType + "\"}";
                        SendTo(msgCount.UserId, message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        public  void SendTaskCountToUsersByMessage(int tblMessageId)
        {

            try
            {
               

                var ptblMessageId = new SqlParameter("@MessageId", tblMessageId);
                var result = _context.Set<MessageCountResponse>().FromSqlRaw("exec Pro_Get_Message_Count_By_Message @MessageId", ptblMessageId)
                    .Where(w => wsClientList.Select(u => u.UserId).Contains(w.UserId)).ToList();

                if (result.Count > 0)
                {
                    foreach (var msgCount in result)
                    {
                        string message = "{\"Type\":\"task_count\",\"TaskCount\":" + msgCount.TaskCount + ",\"Icon\":\"" + Icon + "\",\"Description\":\"" + HttpUtility.JavaScriptStringEncode(msgCount.MessageText) + "\",\"MessageType\":\"" + msgCount.MessageType + "\"}";
                        SendTo(msgCount.UserId, message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
