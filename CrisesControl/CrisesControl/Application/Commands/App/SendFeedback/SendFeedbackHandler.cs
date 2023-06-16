using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.SendFeedback
{
    public class SendFeedbackHandler : IRequestHandler<SendFeedbackRequest, SendFeedbackResponse>
    {
        private readonly IAppQuery _appQuery;
        private readonly ILogger<SendFeedbackHandler> _logger;
        public SendFeedbackHandler(IAppQuery appQuery, ILogger<SendFeedbackHandler> logger)
        {
            this._appQuery = appQuery;
            this._logger = logger;
        }
        public async Task<SendFeedbackResponse> Handle(SendFeedbackRequest request, CancellationToken cancellationToken)
        {
            var result = await _appQuery.SendFeedback(request);
            return result;
        }
    }
}
