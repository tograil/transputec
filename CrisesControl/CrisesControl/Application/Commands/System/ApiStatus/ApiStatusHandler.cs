using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.ApiStatus
{
    public class ApiStatusHandler : IRequestHandler<ApiStatusRequest, ApiStatusResponse>
    {
        private readonly ISystemQuery _systemQuery;
        private readonly ILogger<ApiStatusHandler> _logger;

        public ApiStatusHandler(ISystemQuery systemQuery, ILogger<ApiStatusHandler> logger)
        {
            this._logger = logger;
            this._systemQuery = systemQuery;

        }
        public async Task<ApiStatusResponse> Handle(ApiStatusRequest request, CancellationToken cancellationToken)
        {
            var result = await _systemQuery.ApiStatus(request);
            return result;
        }
    }
}
