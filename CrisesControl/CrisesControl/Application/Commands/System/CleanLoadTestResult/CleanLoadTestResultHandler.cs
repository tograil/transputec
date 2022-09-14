using CrisesControl.Api.Application.Query;
using CrisesControl.Core.System.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.CleanLoadTestResult
{
    public class CleanLoadTestResultHandler : IRequestHandler<CleanLoadTestResultRequest, CleanLoadTestResultResponse>
    {
        private readonly ISystemQuery _systemQuery;
        private readonly ILogger<CleanLoadTestResultHandler> _logger;

        public CleanLoadTestResultHandler(ISystemQuery systemQuery, ILogger<CleanLoadTestResultHandler> logger)
        {
            this._logger = logger;
            this._systemQuery = systemQuery;

        }
        public async Task<CleanLoadTestResultResponse> Handle(CleanLoadTestResultRequest request, CancellationToken cancellationToken)
        {
           var result= await _systemQuery.CleanLoadTestResult(request);
           return result;
        }
    }
}
