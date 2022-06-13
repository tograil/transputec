using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.Index
{
    public class IndexHandler : IRequestHandler<IndexRequest, IndexResponse>
    {
        private readonly ILogger<IndexHandler> _logger;
        private readonly IRegisterQuery _registerQuery;
        public IndexHandler(ILogger<IndexHandler> logger, IRegisterQuery registerQuery)
        {
            _logger = logger;
            _registerQuery = registerQuery;
        }
        public async Task<IndexResponse> Handle(IndexRequest request, CancellationToken cancellationToken)
        {
            var result = await _registerQuery.Index(request);
            return result;
        }
    }
}
