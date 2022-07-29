using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetIcons
{
    public class GetIconsHandler : IRequestHandler<GetIconsRequest, GetIconsResponse>
    {
        private readonly ILookupQuery _lookupQuery;
        private readonly ILogger<GetIconsHandler> _logger;
        public GetIconsHandler(ILogger<GetIconsHandler> logger, ILookupQuery lookupQuery)
        {
            this._logger = logger;
            this._lookupQuery = lookupQuery;
        }
        public async Task<GetIconsResponse> Handle(GetIconsRequest request, CancellationToken cancellationToken)
        {
            var result = await _lookupQuery.GetIcons(request);
            return result;
        }
    }
}
