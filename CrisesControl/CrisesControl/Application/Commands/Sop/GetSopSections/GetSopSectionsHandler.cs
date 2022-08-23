using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.GetSopSections
{
    public class GetSopSectionsHandler : IRequestHandler<GetSopSectionsRequest, GetSopSectionsResponse>
    {
        private ISopQuery _sopQuery;
        private readonly ILogger<GetSopSectionsHandler> _logger;
        public GetSopSectionsHandler(ISopQuery sopQuery, ILogger<GetSopSectionsHandler> logger)
        {
            _sopQuery = sopQuery;
            _logger = logger;
        }
        public async Task<GetSopSectionsResponse> Handle(GetSopSectionsRequest request, CancellationToken cancellationToken)
        {
            var result = await _sopQuery.GetSopSections(request);
            return result;
        }
    }
}
