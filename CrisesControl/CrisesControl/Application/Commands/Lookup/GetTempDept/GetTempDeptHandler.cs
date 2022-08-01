using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetTempDept
{
    public class GetTempDeptHandler : IRequestHandler<GetTempDeptRequest, GetTempDeptResponse>
    {
        private readonly ILookupQuery _lookupQuery;
        private readonly ILogger<GetTempDeptHandler> _logger;
        public GetTempDeptHandler(ILogger<GetTempDeptHandler> logger, ILookupQuery lookupQuery)
        {
            this._logger = logger;
            this._lookupQuery = lookupQuery;
        }
        public async Task<GetTempDeptResponse> Handle(GetTempDeptRequest request, CancellationToken cancellationToken)
        {
            var result = await _lookupQuery.GetTempDept(request);
            return result;
        }
    }
}
