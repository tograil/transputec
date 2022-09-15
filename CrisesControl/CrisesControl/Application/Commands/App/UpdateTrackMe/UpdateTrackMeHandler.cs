using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.UpdateTrackMe
{
    public class UpdateTrackMeHandler : IRequestHandler<UpdateTrackMeRequest, UpdateTrackMeResponse>
    {
        private readonly IAppQuery _appQuery;
        private readonly ILogger<UpdateTrackMeHandler> _logger;
        public UpdateTrackMeHandler(IAppQuery appQuery, ILogger<UpdateTrackMeHandler> logger)
        {
            this._appQuery = appQuery;
            this._logger = logger;
        }
        public async Task<UpdateTrackMeResponse> Handle(UpdateTrackMeRequest request, CancellationToken cancellationToken)
        {
            var result = await _appQuery.UpdateTrackMe(request);
            return result;
        }
    }
}
