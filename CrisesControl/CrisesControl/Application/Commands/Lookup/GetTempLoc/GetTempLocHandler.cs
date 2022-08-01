using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetTempLoc
{
    public class GetTempLocHandler : IRequestHandler<GetTempLocRequest, GetTempLocResponse>
    {
        private readonly ILookupQuery _lookupQuery;
        private readonly ILogger<GetTempLocHandler> _logger;
        public GetTempLocHandler(ILogger<GetTempLocHandler> logger, ILookupQuery lookupQuery)
        {
            this._logger = logger;
            this._lookupQuery = lookupQuery;
        }
        public async Task<GetTempLocResponse> Handle(GetTempLocRequest request, CancellationToken cancellationToken)
        {
            var result = await _lookupQuery.GetTempLoc(request);
            return result;
        }
    }
}
