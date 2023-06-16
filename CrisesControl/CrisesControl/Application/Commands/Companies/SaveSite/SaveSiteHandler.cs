using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.SaveSite
{
    public class SaveSiteHandler : IRequestHandler<SaveSiteRequest, SaveSiteResponse>
    {
        private readonly ICompanyQuery _companyQuery;
        private readonly ILogger<SaveSiteHandler> _logger;
    
        public SaveSiteHandler(ICompanyQuery companyQuery, ILogger<SaveSiteHandler> logger)
        {
            this._companyQuery = companyQuery;
            this._logger = logger;
           

        }
        public async Task<SaveSiteResponse> Handle(SaveSiteRequest request, CancellationToken cancellationToken)
        {
            var site = await _companyQuery.SaveSite(request);

            return site;
        }
    }
}
