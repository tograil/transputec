using CrisesControl.Api.Application.Helpers;
using CrisesControl.Config;
using CrisesControl.Core.AuditLog;
using CrisesControl.Core.AuditLog.Services;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Options;

namespace CrisesControl.Api.Application.Behaviours
{
    public class AuditLogBehaviour<TRequest, TResponse> 
        : IPipelineBehavior<TRequest, TResponse> where TRequest: IRequest<TResponse>
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUser _currentUser;
        private readonly AuditLogOptions _options;

        public AuditLogBehaviour(IAuditLogService auditLogService, ICurrentUser currentUser, IOptions<AuditLogOptions> options)
        {
            _auditLogService = auditLogService;
            _currentUser = currentUser;
            _options = options.Value;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();

            if (!_options.Active)
                return response;

            var savedData = _auditLogService.GetSaveChangesAudit();
            var userId = _currentUser.UserId;
            var companyId = _currentUser.CompanyId;

            var auditLogEntry = new AuditLogEntry
            {
                CompanyId = companyId,
                UserId = userId,
                RequestName = request.GetType().Name,
                SaveChangesAuditCollection = savedData.ToArray(),
                Request = request,
                Response = response
            };

            await _auditLogService.SaveAuditData(auditLogEntry);

            return response;
        }
    }
}