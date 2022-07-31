using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetTempUser
{
    public class GetTempUserHandler : IRequestHandler<GetTempUserRequest, GetTempUserResponse>
    {
        private readonly ILookupQuery _lookupQuery;
        private readonly ILogger<GetTempUserHandler> _logger;
        public GetTempUserHandler(ILogger<GetTempUserHandler> logger, ILookupQuery lookupQuery)
        {
            this._logger = logger;
            this._lookupQuery = lookupQuery;
        }
        public async Task<GetTempUserResponse> Handle(GetTempUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _lookupQuery.GetTempUser(request);
            return result;
        }
    }
}
