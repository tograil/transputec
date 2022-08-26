using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.CaptureUserLocation
{
    public class CaptureUserLocationHandler : IRequestHandler<CaptureUserLocationRequest, CaptureUserLocationResponse>
    {
        private readonly IAppQuery _appQuery;
        private readonly ILogger<CaptureUserLocationHandler> _logger;
        public CaptureUserLocationHandler(IAppQuery appQuery, ILogger<CaptureUserLocationHandler> logger)
        {
            this._appQuery = appQuery;
            this._logger = logger;
        }
        public async Task<CaptureUserLocationResponse> Handle(CaptureUserLocationRequest request, CancellationToken cancellationToken)
        {
            var result = await _appQuery.CaptureUserLocation(request);
            return result;
        }
    }
}
