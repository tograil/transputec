using CrisesControl.Api.Application.Commands.CCWebSocket.Get;
using CrisesControl.Api.Application.Commands.CCWebSocket.ProcessWebsocketSession;
using CrisesControl.Core.CCWebSocket.Repositories;
using CrisesControl.Infrastructure.Services;

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

        public async Task<bool> ProcessWebsocketSession(ProcessWebsocketSessionRequest request)
        {
            try
            {
                 
               if (request.Context != null)
                {
                    await _webSocketRepository.ProcessWebsocketSession(request.Context, request.WebSocket);
                    return true;
                }             

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
    }
}
