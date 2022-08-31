using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.CustomEventLog.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.SaveLogMessage
{
    public class SaveLogMessageHandler:IRequestHandler<SaveLogMessageRequest, SaveLogMessageResponse>
    {
        private readonly ICustomEventLogRepository _customEventLogRepository;
        private readonly IMapper _mapper;
        public SaveLogMessageHandler(ICustomEventLogRepository customEventLogRepository, IMapper mapper)
        {
            _customEventLogRepository = customEventLogRepository;
            _mapper = mapper;
        }

        public async Task<SaveLogMessageResponse> Handle(SaveLogMessageRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SaveLogMessageRequest));
            var result = await _customEventLogRepository.SaveLogMessage(request.EventLogId, request.MessageId, request.MessageText, request.UserId, request.TimeZoneId);
            var response = _mapper.Map<SaveLogMessageResponse>(result);
            return response;
        }
    }
}
