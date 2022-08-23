using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.LibraryTextModel
{
    public class LibraryTextModelHandler : IRequestHandler<LibraryTextModelRequest, LibraryTextModelResponse>
    {
        private readonly ISopQuery _sopQuery;
        private readonly ILogger<LibraryTextModelHandler> _logger;
        public LibraryTextModelHandler(ISopQuery sopQuery, ILogger<LibraryTextModelHandler> logger)
        {
            this._sopQuery = sopQuery;
            this._logger = logger;
        }
        public async Task<LibraryTextModelResponse> Handle(LibraryTextModelRequest request, CancellationToken cancellationToken)
        {
            var result = await _sopQuery.LibraryTextModel(request);
            return result;
        }
    }
}
