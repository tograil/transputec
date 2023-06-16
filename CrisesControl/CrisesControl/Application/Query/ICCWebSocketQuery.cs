using CrisesControl.Api.Application.Commands.CCWebSocket.Get;
using CrisesControl.Api.Application.Commands.CCWebSocket.ProcessWebsocketSession;

namespace CrisesControl.Api.Application.Query
{
    public interface ICCWebSocketQuery
    {
        Task Get(GetRequest request);
        Task<bool> ProcessWebsocketSession(ProcessWebsocketSessionRequest request);


    }
}
