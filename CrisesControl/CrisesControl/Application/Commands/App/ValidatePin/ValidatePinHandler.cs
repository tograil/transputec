using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.ValidatePin
{
    public class ValidatePinHandler : IRequestHandler<ValidatePinRequest, ValidatePinResponse>
    {
        private readonly IAppQuery _appQuery;
        private readonly ILogger<ValidatePinHandler> _logger;
        public ValidatePinHandler(IAppQuery appQuery, ILogger<ValidatePinHandler> logger)
        {
            this._appQuery = appQuery;
            this._logger = logger;
        }
        public async Task<ValidatePinResponse> Handle(ValidatePinRequest request, CancellationToken cancellationToken)
        {
            var result = await _appQuery.ValidatePin(request);
            return result;
        }
    }
}
