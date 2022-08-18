using CrisesControl.Api.Application.Commands.CCWebSocket.Get;
using CrisesControl.Core.CCWebSocket.Repositories;

namespace CrisesControl.Api.Application.Query
{
    public class WebSocketQuery: ICCWebSocketQuery
    {
        private readonly ICCWebSocketRepository _webSocketRepository;
     
        public WebSocketQuery(ICCWebSocketRepository webSocketRepository)
        {
            _webSocketRepository = webSocketRepository;
        }

        public async Task Get(GetRequest request)
        {
            try
            {
             await  _webSocketRepository.Echo(request.WebSocket);         

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
