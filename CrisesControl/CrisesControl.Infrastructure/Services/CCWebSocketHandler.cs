using CrisesControl.Core.CCWebSocket.Repositories;
using CrisesControl.Core.Messages;
using Microsoft.AspNet.SignalR.WebSockets;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public class CCWebSocketHandler : WebSocketHandler
    {
        public CCWebSocketHandler(int? maxIncomingMessageSize) : base(maxIncomingMessageSize)
        {
        }

        public override void OnOpen()
        {
            CCWebSocketHelper.SetClientUserId(this);
            CCWebSocketHelper.AddToClientList(CCWebSocketHelper.UserId, this);
        }

        public override void OnMessage(string message)
        {
            MessageData msgData = JsonConvert.DeserializeObject<MessageData>(message);
            CCWebSocketHelper.SendTo(msgData.UserId, msgData.Message);
        }

        public override void OnClose()
        {
            base.OnClose();
            CCWebSocketHelper.SetClientUserId(this);
            CCWebSocketHelper.RemoveFromClientList(CCWebSocketHelper.UserId, this);
        }
    }
}
