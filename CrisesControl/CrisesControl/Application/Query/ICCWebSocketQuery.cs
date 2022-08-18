using CrisesControl.Api.Application.Commands.CCWebSocket.Get;

namespace CrisesControl.Api.Application.Query
{
    public interface ICCWebSocketQuery
    {
        Task Get(GetRequest request);
    }
}
