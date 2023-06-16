using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageFeatures
{
    public class GetCompanyPackageFeaturesHandler : IRequestHandler<GetCompanyPackageFeaturesRequest, GetCompanyPackageFeaturesResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<GetCompanyPackageFeaturesHandler> _logger;
      
        public GetCompanyPackageFeaturesHandler(IAdminQuery adminQuery, ILogger<GetCompanyPackageFeaturesHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
            
        }
        public async Task<GetCompanyPackageFeaturesResponse> Handle(GetCompanyPackageFeaturesRequest request, CancellationToken cancellationToken)
        {
            var result = await _adminQuery.GetCompanyPackageFeatures(request);
            return result;
        }
    }
}
