using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetUndeliveredMessage
{
    public class GetUndeliveredMessageHandler : IRequestHandler<GetUndeliveredMessageRequest, GetUndeliveredMessageResponse>
    {
        private readonly ILogger<GetUndeliveredMessageHandler> _logger;
        private readonly IReportsQuery _reportsQuery;
        public GetUndeliveredMessageHandler(ILogger<GetUndeliveredMessageHandler> logger, IReportsQuery reportsQuery)
        {
            this._logger = logger;
            this._reportsQuery = reportsQuery;
        }
        public async Task<GetUndeliveredMessageResponse> Handle(GetUndeliveredMessageRequest request, CancellationToken cancellationToken)
        {
            var result = await _reportsQuery.GetUndeliveredMessage(request);
            return result;
        }
    }
}
