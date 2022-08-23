using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.GetTagList
{
    public class GetTagListHandler : IRequestHandler<GetTagListRequest, GetTagListResponse>
    {
        private ISopQuery _sopQuery;
        private readonly ILogger<GetTagListHandler> _logger;
        public GetTagListHandler(ISopQuery sopQuery, ILogger<GetTagListHandler> logger)
        {
            _sopQuery = sopQuery;
            _logger = logger;
        }
        public async Task<GetTagListResponse> Handle(GetTagListRequest request, CancellationToken cancellationToken)
        {
            var result = await _sopQuery.GetTagList(request);
            return result;
        }
    }
}
