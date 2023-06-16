using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetSocialIntegration
{
    public class GetSocialIntegrationHandler : IRequestHandler<GetSocialIntegrationRequest, GetSocialIntegrationResponse>
    {
        private readonly ICompanyQuery _companyQuery;
        private readonly ILogger<GetSocialIntegrationHandler> _logger;
        private readonly GetSocialIntegrationValidator _getSocialIntegrationValidator;
        public GetSocialIntegrationHandler(ICompanyQuery companyQuery, ILogger<GetSocialIntegrationHandler> logger, GetSocialIntegrationValidator getSocialIntegrationValidator)
        {
            this._companyQuery = companyQuery;
            this._logger = logger;
            this._getSocialIntegrationValidator = getSocialIntegrationValidator;

        }
        public async Task<GetSocialIntegrationResponse> Handle(GetSocialIntegrationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetSocialIntegrationRequest));

            await _getSocialIntegrationValidator.ValidateAndThrowAsync(request, cancellationToken);

            var socials = await _companyQuery.GetSocialIntegration(request);

            return socials;
        }
    }
}
