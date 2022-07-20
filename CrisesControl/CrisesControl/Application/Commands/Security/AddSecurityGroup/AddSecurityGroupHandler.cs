using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.AddSecurityGroup
{
    public class AddSecurityGroupHandler : IRequestHandler<AddSecurityGroupRequest, AddSecurityGroupResponse>
    {
        private readonly ISecurityQuery _securityQuery;
        private readonly ILogger<AddSecurityGroupHandler> _logger;
        public AddSecurityGroupHandler(ISecurityQuery securityQuery, ILogger<AddSecurityGroupHandler> logger)
        {
            this._logger = logger;
            this._securityQuery = securityQuery;

        }
        public async Task<AddSecurityGroupResponse> Handle(AddSecurityGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _securityQuery.AddSecurityGroup(request);
            return result;
        }
    }
}
