﻿using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.AuditLog;
using CrisesControl.Core.AuditLog.Services;
using MediatR;
using MediatR.Pipeline;

namespace CrisesControl.Api.Application.Behaviours
{
    public class DomainLogBehaviour<TRequest, TResponse> 
        : IPipelineBehavior<TRequest, TResponse> where TRequest: IRequest<TResponse>
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUser _currentUser;

        public DomainLogBehaviour(IAuditLogService auditLogService, ICurrentUser currentUser)
        {
            _auditLogService = auditLogService;
            _currentUser = currentUser;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();

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

            //throw new NotImplementedException();
        }
    }
}