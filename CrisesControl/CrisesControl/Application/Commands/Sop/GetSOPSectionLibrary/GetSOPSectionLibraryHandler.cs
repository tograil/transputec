using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.GetSOPSectionLibrary
{
    public class GetSOPSectionLibraryHandler : IRequestHandler<GetSOPSectionLibraryRequest, GetSOPSectionLibraryResponse>
    {
        private ISopQuery _sopQuery;
        private readonly ILogger<GetSOPSectionLibraryHandler> _logger;

        public GetSOPSectionLibraryHandler(ISopQuery sopQuery, ILogger<GetSOPSectionLibraryHandler> logger)
        {
            _sopQuery = sopQuery;
            _logger = logger;
        }
        public async Task<GetSOPSectionLibraryResponse> Handle(GetSOPSectionLibraryRequest request, CancellationToken cancellationToken)
        {
            var result = await _sopQuery.GetSOPSectionLibrary(request);
            return result;
        }
    }
}
