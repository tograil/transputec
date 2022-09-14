using CrisesControl.Core.CCWebSocket;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CrisesControl.Infrastructure.Services
{
    public static class CCWebSocketHelper
    {
        public static int UserId = 0;
        public static List<SocketClientList> wsClientList = new List<SocketClientList>();
        static readonly string Icon = "assets/images/messages-icon.png";
        private static readonly CrisesControlContext _context;
        private static readonly IHttpContextAccessor _httpContextAccessor;
        public static void SetClientUserId(CCWebSocketHandler session)
        {
            
            string sesPath = _httpContextAccessor.HttpContext.Session.Id;
            
            //string[] pathParts = sesPath.Trim('/').Split('/');
            //if (pathParts.Length >= 2) {
            //if (pathParts[0] == "portalconnect") {
            int.TryParse(sesPath, out UserId);
            //}
            //}
        }

        public static void AddToClientList(int clientUserId, CCWebSocketHandler session)
        {
            var userClient = wsClientList.Where(s => s.UserId == clientUserId).Any();
            //if (!userClient) {
            wsClientList.Add(new SocketClientList { UserId = clientUserId, CCWebSocketHandle = session });
            //}
        }

        public static void SendTo(int userId, string message)
        {
            var userClient = wsClientList.Where(s => s.UserId == userId).ToList();
            foreach (var client in userClient)
            {
                if (client != null)
                {
                    client.CCWebSocketHandle.Send(message);
                }
            }
        }

        public static void RemoveFromClientList(int clientUserId, CCWebSocketHandler session)
        {
            var userClient = wsClientList.Where(s => s.UserId == clientUserId).ToList();
            foreach (var client in userClient)
            {
               
                        wsClientList.Remove(client);
                  
            }
        }


        public static void SendCountToUsersByMessage(int tblMessageId)
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

        public static void SendMessageCountToUsersByMessage(int tblMessageId)
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

        

        public static void SendTaskCountToUsersByMessage(int tblMessageId)
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
