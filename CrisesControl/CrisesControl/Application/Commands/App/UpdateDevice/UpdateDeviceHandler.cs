using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.UpdateDevice
{
    public class UpdateDeviceHandler : IRequestHandler<UpdateDeviceRequest, UpdateDeviceResponse>
    {
        private readonly IAppQuery _appQuery;
        private readonly ILogger<UpdateDeviceHandler> _logger;
        public UpdateDeviceHandler(IAppQuery appQuery, ILogger<UpdateDeviceHandler> logger)
        {
            this._appQuery = appQuery;
            this._logger = logger;
        }
        public async Task<UpdateDeviceResponse> Handle(UpdateDeviceRequest request, CancellationToken cancellationToken)
        {
            var result = await _appQuery.UpdateDevice(request);
            return result;
        }
    }
}
