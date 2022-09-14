using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.GetAuditLogsByRecordId
{
    public class GetAuditLogsByRecordIdHandler : IRequestHandler<GetAuditLogsByRecordIdRequest, GetAuditLogsByRecordIdResponse>
    {
        private readonly ISystemQuery _systemQuery;
        private readonly ILogger<GetAuditLogsByRecordIdHandler> _logger;

        public GetAuditLogsByRecordIdHandler(ISystemQuery systemQuery, ILogger<GetAuditLogsByRecordIdHandler> logger)
        {
            this._logger = logger;
            this._systemQuery = systemQuery;

        }
        public async Task<GetAuditLogsByRecordIdResponse> Handle(GetAuditLogsByRecordIdRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAuditLogsByRecordIdRequest));
            var result = await _systemQuery.GetAuditLogsByRecordId(request);
            return result;
        }
    }
}
