using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Academy.GetUserVideos
{
    public class GetUserVideosHandler : IRequestHandler<GetUserVideosRequest, GetUserVideosResponse>
    {
        private readonly IAcademyQuery _academyQuery;
        private readonly ILogger<GetUserVideosHandler> _logger;

        public GetUserVideosHandler(IAcademyQuery academyQuery, ILogger<GetUserVideosHandler> logger)
        {
            this._academyQuery = academyQuery;
            this._logger = logger;

        }
        public async Task<GetUserVideosResponse> Handle(GetUserVideosRequest request, CancellationToken cancellationToken)
        {
            var result = await _academyQuery.GetUserVideos(request);
            return result;
        }
    }
}
