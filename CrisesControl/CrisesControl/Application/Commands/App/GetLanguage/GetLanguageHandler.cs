using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.GetLanguage
{
    public class GetLanguageHandler : IRequestHandler<GetLanguageRequest, GetLanguageResponse>
    {
        private readonly IAppQuery _appQuery;
        private readonly ILogger<GetLanguageHandler> _logger;
        public GetLanguageHandler(IAppQuery appQuery, ILogger<GetLanguageHandler> logger)
        {
            this._appQuery = appQuery;
            this._logger = logger;
        }
        public async Task<GetLanguageResponse> Handle(GetLanguageRequest request, CancellationToken cancellationToken)
        {
            var result = await _appQuery.GetLanguage(request);
            return result;
        }
    }
}
