using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncidentType
{
    public class DeleteLibIncidentTypeHandler : IRequestHandler<DeleteLibIncidentTypeRequest, DeleteLibIncidentTypeResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<DeleteLibIncidentTypeHandler> _logger;
        private readonly DeleteLibIncidentTypeValidator _deleteLibIncidentValidator;
        public DeleteLibIncidentTypeHandler(IAdminQuery adminQuery, ILogger<DeleteLibIncidentTypeHandler> logger, DeleteLibIncidentTypeValidator deleteLibIncidentValidator)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
            this._deleteLibIncidentValidator = deleteLibIncidentValidator;
        }
        public async Task<DeleteLibIncidentTypeResponse> Handle(DeleteLibIncidentTypeRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteLibIncidentTypeRequest));

            await _deleteLibIncidentValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _adminQuery.DeleteLibIncidentType(request);
            return result;
        }
    }
}
