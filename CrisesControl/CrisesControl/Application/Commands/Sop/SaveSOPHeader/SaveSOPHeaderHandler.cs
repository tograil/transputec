using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.SaveSOPHeader
{
    public class SaveSOPHeaderHandler : IRequestHandler<SaveSOPHeaderRequest, SaveSOPHeaderResponse>
    {
        private readonly ISopQuery _sopQuery;
        private readonly ILogger<SaveSOPHeaderHandler> _logger;
        public SaveSOPHeaderHandler(ISopQuery sopQuery, ILogger<SaveSOPHeaderHandler> logger)
        {
            _sopQuery = sopQuery;
            _logger = logger;
        }
        public async Task<SaveSOPHeaderResponse> Handle(SaveSOPHeaderRequest request, CancellationToken cancellationToken)
        {
            var result = await _sopQuery.SaveSOPHeader(request);
            return result;
        }
    }
}
