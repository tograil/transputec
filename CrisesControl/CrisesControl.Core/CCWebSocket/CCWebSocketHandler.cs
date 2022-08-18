
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.CCWebSocket
{
    public class CCWebSocketHandler : WebSocket
    {


        //public override void OnOpen()
        //{
        //    CCWebSocketHelper.SetClientUserId(this);
        //    CCWebSocketHelper.AddToClientList(CCWebSocketHelper.UserId, this);
        //}

        //public override void OnMessage(string message)
        //{
        //    MessageData msgData = JsonConvert.DeserializeObject<MessageData>(message);
        //    CCWebSocketHelper.SendTo(msgData.UserId, msgData.Message);
        //}

        //public override void OnClose()
        //{
        //    base.OnClose();
        //    CCWebSocketHelper.SetClientUserId(this);
        //    CCWebSocketHelper.RemoveFromClientList(CCWebSocketHelper.UserId, this);
        //}
        public override WebSocketCloseStatus? CloseStatus => throw new NotImplementedException();

        public override string? CloseStatusDescription => throw new NotImplementedException();

        public override WebSocketState State => throw new NotImplementedException();

        public override string? SubProtocol => throw new NotImplementedException();

        public override void Abort()
        {
            throw new NotImplementedException();
        }

        public override Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
