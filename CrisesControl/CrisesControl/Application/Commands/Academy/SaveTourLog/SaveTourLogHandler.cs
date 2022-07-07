using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Academy.SaveTourLog
{
    public class SaveTourLogHandler : IRequestHandler<SaveTourLogRequest, SaveTourLogResponse>
    {
        private readonly IAcademyQuery _academyQuery;
        private readonly ILogger<SaveTourLogHandler> _logger;

        public SaveTourLogHandler(IAcademyQuery academyQuery, ILogger<SaveTourLogHandler> logger)
        {
            this._academyQuery = academyQuery;
            this._logger = logger;

        }
        public async Task<SaveTourLogResponse> Handle(SaveTourLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _academyQuery.SaveTourLog(request);
            return result;
        }
    }
}
