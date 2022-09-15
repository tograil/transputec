using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CCWebSocket
{
    public class SocketList
    {
        public int UserId { get; set; }
        [NotMapped]
        public WebSocketSession CCWebSocketSession { get; set; }
    }
}
