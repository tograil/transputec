using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.GetTnC
{
    public class GetTnCHandler : IRequestHandler<GetTnCRequest, GetTnCResponse>
    {
        private readonly IAppQuery _appQuery;
        private readonly ILogger<GetTnCHandler> _logger;
        public GetTnCHandler(IAppQuery appQuery, ILogger<GetTnCHandler> logger)
        {
            this._appQuery = appQuery;
            this._logger = logger;
        }
        public async Task<GetTnCResponse> Handle(GetTnCRequest request, CancellationToken cancellationToken)
        {
            var result = await _appQuery.GetTnC(request);
            return result;
        }
    }
}
