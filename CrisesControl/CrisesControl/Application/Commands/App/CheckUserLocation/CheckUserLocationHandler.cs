using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.CheckUserLocation
{
    public class CheckUserLocationHandler : IRequestHandler<CheckUserLocationRequest, CheckUserLocationResponse>
    {
        private readonly IAppQuery _appQuery;
        private readonly ILogger<CheckUserLocationHandler> _logger;
        public CheckUserLocationHandler(IAppQuery appQuery, ILogger<CheckUserLocationHandler> logger)
        {
            this._appQuery = appQuery;
            this._logger = logger;
        }
        public async Task<CheckUserLocationResponse> Handle(CheckUserLocationRequest request, CancellationToken cancellationToken)
        {
            var result = await _appQuery.CheckUserLocation(request);
            return result;
        }
    }
}
