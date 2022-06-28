using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetSite
{
    public class GetSiteHandler : IRequestHandler<GetSiteRequest, GetSiteResponse>
    {
        private readonly ICompanyQuery _companyQuery;
        private readonly ILogger<GetSiteHandler> _logger;
        private readonly GetSiteValidator _getSiteValidator;
        public GetSiteHandler(ICompanyQuery companyQuery, ILogger<GetSiteHandler> logger, GetSiteValidator getSiteValidator)
        {
            this._companyQuery = companyQuery;
            this._logger = logger;
            this._getSiteValidator = getSiteValidator;

        }
        public async Task<GetSiteResponse> Handle(GetSiteRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetSiteRequest));

            await _getSiteValidator.ValidateAndThrowAsync(request, cancellationToken);

            var sites = await _companyQuery.GetSite(request);

            return sites;
        }
    }
}
