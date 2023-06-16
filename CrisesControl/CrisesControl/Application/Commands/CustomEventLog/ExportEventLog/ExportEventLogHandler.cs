using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.CustomEventLog;
using CrisesControl.Core.CustomEventLog.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.ExportEventLog
{
    public class ExportEventLogHandler:IRequestHandler<ExportEventLogRequest, ExportEventLogResponse>
    {
        private readonly ICustomEventLogRepository _customEventLogRepository;
        private readonly IMapper _mapper;
        public ExportEventLogHandler(ICustomEventLogRepository customEventLogRepository, IMapper mapper)
        {
            _customEventLogRepository = customEventLogRepository;
            _mapper = mapper;
        }

        public async Task<ExportEventLogResponse> Handle(ExportEventLogRequest request, CancellationToken cancellationToken)
        {
            var mappedRequest = _mapper.Map<EventLogModel>(request);
            var result = await _customEventLogRepository.ExportEventLog(mappedRequest); 
            var response = _mapper.Map<ExportEventLogResponse>(result);
            return response;
        }
    }
}
