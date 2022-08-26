using Ardalis.GuardClauses;
using CrisesControl.Core.CustomEventLog.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.SaveEventLogHeader
{
    public class SaveEventLogHeaderHandler : IRequestHandler<SaveEventLogHeaderRequest, SaveEventLogHeaderResponse>
    {
        private readonly ICustomEventLogRepository _customEventLogRepository;
        public SaveEventLogHeaderHandler(ICustomEventLogRepository customEventLogRepository)
        {
            _customEventLogRepository = customEventLogRepository;
        }
        public async Task<SaveEventLogHeaderResponse> Handle(SaveEventLogHeaderRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SaveEventLogHeaderRequest));
            var response = new SaveEventLogHeaderResponse();
            response.Result = await _customEventLogRepository.SaveEventLogHeader(request.ActiveIncidentId, request.PermittedDepartment, request.CompanyId, request.UserId, request.TimeZoneId);
            return response;
        }
    }
}
