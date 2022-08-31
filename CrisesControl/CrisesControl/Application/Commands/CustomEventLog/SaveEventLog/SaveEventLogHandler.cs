using Ardalis.GuardClauses;
using CrisesControl.Core.CustomEventLog.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.SaveEventLog
{
    public class SaveEventLogHandler : IRequestHandler<SaveEventLogRequest, SaveEventLogResponse>
    {
        private readonly ICustomEventLogRepository _customEventLogRepository;
        public SaveEventLogHandler(ICustomEventLogRepository customEventLogRepository)
        {
            _customEventLogRepository = customEventLogRepository;
        }
        public async Task<SaveEventLogResponse> Handle(SaveEventLogRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SaveEventLogRequest));
            var response = new SaveEventLogResponse();
            response.EventLogId = await _customEventLogRepository.SaveEventLog(request.IP, request.UserID, request.TimeZoneId);
            return response;
        }
    }
}
