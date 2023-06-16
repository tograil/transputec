using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.SendCustomerNotice
{
    public class SendCustomerNoticeHandler : IRequestHandler<SendCustomerNoticeRequest, SendCustomerNoticeResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<SendCustomerNoticeHandler> _logger;
        public SendCustomerNoticeHandler(IAdminQuery adminQuery, ILogger<SendCustomerNoticeHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
        }
        public async Task<SendCustomerNoticeResponse> Handle(SendCustomerNoticeRequest request, CancellationToken cancellationToken)
        {
            var result = await _adminQuery.SendCustomerNotice(request);
            return result;
        }
    }
}
