using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.TwilioLogDump
{
    public class TwilioLogDumpHandler : IRequestHandler<TwilioLogDumpRequest, TwilioLogDumpResponse>
    {
        private readonly ISystemQuery _systemQuery;
        private readonly ILogger<TwilioLogDumpHandler> _logger;

        public TwilioLogDumpHandler(ISystemQuery systemQuery, ILogger<TwilioLogDumpHandler> logger)
        {
            this._logger = logger;
            this._systemQuery = systemQuery;

        }
        public async Task<TwilioLogDumpResponse> Handle(TwilioLogDumpRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TwilioLogDumpRequest));
            var result = await _systemQuery.TwilioLogDump(request);
            return result;
        }
    }
}
