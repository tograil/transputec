using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.GetSecurityGroup
{
    public class GetSecurityGroupHandler:IRequestHandler<GetSecurityGroupRequest, GetSecurityGroupResponse>
    {
        private readonly ISecurityQuery _securityQuery;
        private readonly ILogger<GetSecurityGroupHandler> _logger;
        private readonly GetSecurityGroupValidator _getSecurityGroupValidator;
        public GetSecurityGroupHandler(ISecurityQuery securityQuery, ILogger<GetSecurityGroupHandler> logger, GetSecurityGroupValidator getSecurityGroupValidator)
        {
            this._logger = logger;
            this._securityQuery = securityQuery;
            this._getSecurityGroupValidator = getSecurityGroupValidator;
        }

        public async Task<GetSecurityGroupResponse> Handle(GetSecurityGroupRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetSecurityGroupRequest));
            await _getSecurityGroupValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _securityQuery.GetSecurityGroup(request);
            return result;
        }
    }
}
