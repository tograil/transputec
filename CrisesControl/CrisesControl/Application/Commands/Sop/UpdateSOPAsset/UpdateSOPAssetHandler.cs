using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.UpdateSOPAsset
{
    public class UpdateSOPAssetHandler : IRequestHandler<UpdateSOPAssetRequest, UpdateSOPAssetResponse>
    {

        private readonly ISopQuery _sopQuery;
        private readonly ILogger<UpdateSOPAssetHandler> _logger;
        public UpdateSOPAssetHandler(ISopQuery sopQuery, ILogger<UpdateSOPAssetHandler> logger)
        {
            this._sopQuery = sopQuery;
            this._logger = logger;
        }
        public async Task<UpdateSOPAssetResponse> Handle(UpdateSOPAssetRequest request, CancellationToken cancellationToken)
        {
            var result = await _sopQuery.UpdateSOPAsset(request);
            return result;
        }
    }
}
