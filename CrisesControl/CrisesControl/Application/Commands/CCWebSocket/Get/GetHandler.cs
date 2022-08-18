using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CCWebSocket.Get
{
    public class GetHandler : IRequestHandler<GetRequest,bool>
    {
        private readonly ICCWebSocketQuery _webSocketQuery;
       public GetHandler(ICCWebSocketQuery webSocketQuery)
        {
            _webSocketQuery = webSocketQuery;  
        }
        public async Task<bool> Handle(GetRequest request, CancellationToken cancellationToken)
        {
           await _webSocketQuery.Get(request);
            return true;
        }
    }
}
