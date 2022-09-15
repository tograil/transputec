using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CCWebSocket.ProcessWebsocketSession
{
    public class ProcessWebsocketSessionHandler : IRequestHandler<ProcessWebsocketSessionRequest, bool>
    {
        private readonly ICCWebSocketQuery _webSocketQuery;
        public ProcessWebsocketSessionHandler(ICCWebSocketQuery webSocketQuery)
        {
            _webSocketQuery = webSocketQuery;
        }
        public async Task<bool> Handle(ProcessWebsocketSessionRequest request, CancellationToken cancellationToken)
        {
           var result= await _webSocketQuery.ProcessWebsocketSession(request);
            return result;
        }
    }
}
