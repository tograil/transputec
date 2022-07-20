using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.SopLibrary.GetSopSections
{
    public class GetSopSectionsHandler : IRequestHandler<GetSopSectionsRequest, GetSopSectionsResponse>
    {
        private readonly ISopLibraryQuery _sopLibraryQuery;
        private readonly ILogger<GetSopSectionsHandler> _logger;
    
        public GetSopSectionsHandler(ISopLibraryQuery sopLibraryQuery, ILogger<GetSopSectionsHandler> logger)
        {
            this._logger = logger;
            this._sopLibraryQuery = sopLibraryQuery;
          
        }
        public async Task<GetSopSectionsResponse> Handle(GetSopSectionsRequest request, CancellationToken cancellationToken)
        {
            var delete = await _sopLibraryQuery.GetSopSections(request);
            return delete;
        }
    }
}
