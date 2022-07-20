using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.GetAllSecurityObjects
{
    public class GetAllSecurityObjectsHandler:IRequestHandler<GetAllSecurityObjectsRequest, GetAllSecurityObjectsResponse>
    {
        private readonly ISecurityQuery _securityQuery;
        private readonly ILogger<GetAllSecurityObjectsHandler> _logger;
        public GetAllSecurityObjectsHandler(ISecurityQuery securityQuery, ILogger<GetAllSecurityObjectsHandler> logger)
        {
            this._logger = logger;
            this._securityQuery = securityQuery;

        }

        public async Task<GetAllSecurityObjectsResponse> Handle(GetAllSecurityObjectsRequest request, CancellationToken cancellationToken)
        {
            var result = await _securityQuery.GetAllSecurityObjects(request);
            return result;
        }
    }
}
