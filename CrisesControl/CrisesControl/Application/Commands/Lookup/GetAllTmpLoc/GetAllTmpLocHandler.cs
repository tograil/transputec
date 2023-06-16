using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetAllTmpLoc
{
    public class GetAllTmpLocHandler : IRequestHandler<GetAllTmpLocRequest, GetAllTmpLocResponse>
    {
        private readonly ILookupQuery _lookupQuery;
        private readonly ILogger<GetAllTmpLocHandler> _logger;
        public GetAllTmpLocHandler(ILogger<GetAllTmpLocHandler> logger, ILookupQuery lookupQuery)
        {
            this._logger = logger;
            this._lookupQuery = lookupQuery;
        }
        public async Task<GetAllTmpLocResponse> Handle(GetAllTmpLocRequest request, CancellationToken cancellationToken)
        {
            var result = await _lookupQuery.GetAllTmpLoc(request);
            return result;
        }
    }
}
