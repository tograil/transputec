using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.DeleteSecurityGroup
{
    public class DeleteSecurityGroupHandler : IRequestHandler<DeleteSecurityGroupRequest, DeleteSecurityGroupResponse>
    {
        private readonly ISecurityQuery _securityQuery;
        private readonly ILogger<DeleteSecurityGroupHandler> _logger;
        private readonly DeleteSecurityGroupValidator _deleteSecurityGroupValidator;
        public DeleteSecurityGroupHandler(ISecurityQuery securityQuery, ILogger<DeleteSecurityGroupHandler> logger, DeleteSecurityGroupValidator deleteSecurityGroupValidator)
        {
            this._logger = logger;
            this._securityQuery = securityQuery;
            this._deleteSecurityGroupValidator = deleteSecurityGroupValidator;

        }
        public async Task<DeleteSecurityGroupResponse> Handle(DeleteSecurityGroupRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteSecurityGroupRequest));
            await _deleteSecurityGroupValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _securityQuery.DeleteSecurityGroup(request);
            return result;
        }
    }
}
