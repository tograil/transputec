using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.UpdateSecurityGroup
{
    public class UpdateSecurityGroupHandler : IRequestHandler<UpdateSecurityGroupRequest, UpdateSecurityGroupResponse>
    {
        private readonly ISecurityQuery _securityQuery;
        private readonly ILogger<UpdateSecurityGroupHandler> _logger;
        public UpdateSecurityGroupHandler(ISecurityQuery securityQuery, ILogger<UpdateSecurityGroupHandler> logger)
        {
            this._logger = logger;
            this._securityQuery = securityQuery;

        }

        public async Task<UpdateSecurityGroupResponse> Handle(UpdateSecurityGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _securityQuery.UpdateSecurityGroup(request);
            return result;
        }
    }
}
