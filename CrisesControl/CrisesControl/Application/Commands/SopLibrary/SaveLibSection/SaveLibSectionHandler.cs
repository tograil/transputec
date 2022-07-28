using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.SopLibrary.SaveLibSection
{
    public class SaveLibSectionHandler:IRequestHandler<SaveLibSectionRequest, SaveLibSectionResponse>
    {
        private readonly ISopLibraryQuery _sopLibraryQuery;
        private readonly ILogger<SaveLibSectionHandler> _logger;

        public SaveLibSectionHandler(ISopLibraryQuery sopLibraryQuery, ILogger<SaveLibSectionHandler> logger)
        {
            this._logger = logger;
            this._sopLibraryQuery = sopLibraryQuery;

        }

        public async Task<SaveLibSectionResponse> Handle(SaveLibSectionRequest request, CancellationToken cancellationToken)
        {
            var saveLib = await _sopLibraryQuery.SaveLibSection(request);
            return saveLib;
        }
    }
}
