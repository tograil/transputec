using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.RemoveSection
{
    public class RemoveSectionHandler : IRequestHandler<RemoveSectionRequest, RemoveSectionResponse>
    {
        private ISopQuery _sopQuery;
        private readonly ILogger<RemoveSectionHandler> _logger;
  
        public RemoveSectionHandler(ISopQuery sopQuery, ILogger<RemoveSectionHandler> logger)
        {
            _sopQuery = sopQuery;
            _logger = logger;
        }
        public async Task<RemoveSectionResponse> Handle(RemoveSectionRequest request, CancellationToken cancellationToken)
        {
            var result = await _sopQuery.RemoveSection(request);
            return result;
        }
    }
}
