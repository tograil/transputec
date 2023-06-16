using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetAllTmpDept
{
    public class GetAllTmpDeptHandler : IRequestHandler<GetAllTmpDeptRequest, GetAllTmpDeptResponse>
    {
        private readonly ILookupQuery _lookupQuery;
        private readonly ILogger<GetAllTmpDeptHandler> _logger;
        public GetAllTmpDeptHandler(ILogger<GetAllTmpDeptHandler> logger, ILookupQuery lookupQuery)
        {
            this._logger = logger;
            this._lookupQuery = lookupQuery;
        }
        public async Task<GetAllTmpDeptResponse> Handle(GetAllTmpDeptRequest request, CancellationToken cancellationToken)
        {
            var result = await _lookupQuery.GetAllTmpDept(request);
            return result;
        }
    }
}
