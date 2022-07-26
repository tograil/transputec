using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.ReorderSection
{
    public class ReorderSectionHandler : IRequestHandler<ReorderSectionRequest, ReorderSectionResponse>
    {
        private ISopQuery _sopQuery;
        private readonly ILogger<ReorderSectionHandler> _logger;

        public ReorderSectionHandler(ISopQuery sopQuery, ILogger<ReorderSectionHandler> logger)
        {
            _sopQuery = sopQuery;
            _logger = logger;
        }
        public Task<ReorderSectionResponse> Handle(ReorderSectionRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
