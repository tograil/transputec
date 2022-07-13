using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.SaveLanguageItem
{
    public class SaveLanguageItemHandler : IRequestHandler<SaveLanguageItemRequest, SaveLanguageItemResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<SaveLanguageItemHandler> _logger;

        public SaveLanguageItemHandler(IAdminQuery adminQuery, ILogger<SaveLanguageItemHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
        }
        public async Task<SaveLanguageItemResponse> Handle(SaveLanguageItemRequest request, CancellationToken cancellationToken)
        {
            var result = await _adminQuery.SaveLanguageItem(request);
            return result;
        }
    }
}
