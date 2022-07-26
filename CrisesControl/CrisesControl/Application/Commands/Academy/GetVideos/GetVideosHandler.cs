using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Academy.GetVideos
{
    public class GetVideosHandler : IRequestHandler<GetVideosRequest, GetVideosResponse>
    {
        private readonly IAcademyQuery _academyQuery;
        private readonly ILogger<GetVideosHandler> _logger;

        public GetVideosHandler(IAcademyQuery academyQuery, ILogger<GetVideosHandler> logger)
        {
            this._academyQuery = academyQuery;
            this._logger = logger;

        }
        public async Task<GetVideosResponse> Handle(GetVideosRequest request, CancellationToken cancellationToken)
        {
            var result = await _academyQuery.GetVideos(request);
            return result;
        }
    }
}
