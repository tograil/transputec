using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.UpdatePackageItem
{
    public class UpdatePackageItemHandler : IRequestHandler<UpdatePackageItemRequest, UpdatePackageItemResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<UpdatePackageItemHandler> _logger;

        public UpdatePackageItemHandler(IAdminQuery adminQuery, ILogger<UpdatePackageItemHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
        }
        public async Task<UpdatePackageItemResponse> Handle(UpdatePackageItemRequest request, CancellationToken cancellationToken)
        {
            var result = await _adminQuery.UpdatePackageItem(request);
            return result;
        }
    }
}
