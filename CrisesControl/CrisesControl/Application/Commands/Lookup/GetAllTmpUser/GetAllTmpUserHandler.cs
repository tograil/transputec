using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetAllTmpUser
{
    public class GetAllTmpUserHandler : IRequestHandler<GetAllTmpUserRequest, GetAllTmpUserResponse>
    {
        private readonly ILookupQuery _lookupQuery;
        private readonly ILogger<GetAllTmpUserHandler> _logger;
        public GetAllTmpUserHandler(ILogger<GetAllTmpUserHandler> logger, ILookupQuery lookupQuery)
        {
            this._logger = logger;
            this._lookupQuery = lookupQuery;
        }
        public async Task<GetAllTmpUserResponse> Handle(GetAllTmpUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _lookupQuery.GetAllTmpUser(request);
            return result;
        }
    }
}
