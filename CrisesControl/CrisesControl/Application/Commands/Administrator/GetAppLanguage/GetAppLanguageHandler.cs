using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetAppLanguage
{
    public class GetAppLanguageHandler : IRequestHandler<GetAppLanguageRequest, GetAppLanguageResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<GetAppLanguageHandler> _logger;

        public GetAppLanguageHandler(IAdminQuery adminQuery, ILogger<GetAppLanguageHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
        }  
        public async Task<GetAppLanguageResponse> Handle(GetAppLanguageRequest request, CancellationToken cancellationToken)
        {
                var result = await _adminQuery.GetAppLanguage(request);
                return result;
        }
    }
}
