using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.SetupCompleted
{
    public class SetupCompletedHandler : IRequestHandler<SetupCompletedRequest, SetupCompletedResponse>
    {
        private readonly ILogger<SetupCompletedHandler> _logger;
        private readonly IRegisterQuery _registerQuery;
        public SetupCompletedHandler(ILogger<SetupCompletedHandler> logger, IRegisterQuery registerQuery)
        {
            this._logger = logger;
            this._registerQuery=registerQuery;
        }   
        public async Task<SetupCompletedResponse> Handle(SetupCompletedRequest request, CancellationToken cancellationToken)
        {
            var result = await _registerQuery.SetupCompleted(request);
            return result;
        }
    }
}
