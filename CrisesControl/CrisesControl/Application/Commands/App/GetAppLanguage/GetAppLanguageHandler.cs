using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.GetAppLanguage
{
    public class GetAppLanguageHandler : IRequestHandler<GetAppLanguageRequest, GetAppLanguageResponse>
    {
        private readonly IAppQuery _appQuery;
        private readonly ILogger<GetAppLanguageHandler> _logger;
        public GetAppLanguageHandler(IAppQuery appQuery, ILogger<GetAppLanguageHandler> logger)
        {
            this._appQuery = appQuery;
            this._logger = logger;
        }
        public async Task<GetAppLanguageResponse> Handle(GetAppLanguageRequest request, CancellationToken cancellationToken)
        {
            var result = await _appQuery.GetAppLanguage(request);
            return result;
        }
    }
}
