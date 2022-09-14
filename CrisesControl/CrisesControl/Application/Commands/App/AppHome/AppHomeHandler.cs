using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.AppHome
{
    public class AppHomeHandler : IRequestHandler<AppHomeRequest, AppHomeResponse>
    {
        private readonly IAppQuery _appQuery;
        private readonly ILogger<AppHomeHandler> _logger;
        public AppHomeHandler(IAppQuery appQuery, ILogger<AppHomeHandler> logger)
        {
            this._appQuery = appQuery;
            this._logger = logger;
        }
        public async Task<AppHomeResponse> Handle(AppHomeRequest request, CancellationToken cancellationToken)
        {
            var result = await _appQuery.AppHome(request);
            return result;
        }
    }
}
