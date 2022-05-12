using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Security;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.GetCompanySecurityGroup
{
    public class GetCompanySecurityGroupHandler : IRequestHandler<GetCompanySecurityGroupRequest, GetCompanySecurityGroupResponse>
    {
        private readonly GetCompanySecurityGroupValidator _securityValidator;
        private readonly ISecurityQuery _securityQuery;
        private readonly ISecurityRepository _securityService;

        public GetCompanySecurityGroupHandler(GetCompanySecurityGroupValidator securityValidator, ISecurityQuery securityQuery, ISecurityRepository securityService)
        {
            this._securityService = securityService; 
            this._securityQuery = securityQuery;
            this._securityValidator = securityValidator;
        }
        public async Task<GetCompanySecurityGroupResponse> Handle(GetCompanySecurityGroupRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetCompanySecurityGroupRequest));

            await _securityValidator.ValidateAndThrowAsync(request, cancellationToken);

            var securities = await _securityQuery.GetCompanySecurityGroup(request);
            return securities;
        }
    }
}
