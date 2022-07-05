using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncident
{
    public class DeleteLibIncidentHandler : IRequestHandler<DeleteLibIncidentRequest, DeleteLibIncidentResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<DeleteLibIncidentHandler> _logger;
        private readonly DeleteLibIncidentValidator _deleteLibIncidentValidator;
        public DeleteLibIncidentHandler(IAdminQuery adminQuery, ILogger<DeleteLibIncidentHandler> logger, DeleteLibIncidentValidator deleteLibIncidentValidator)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
            this._deleteLibIncidentValidator = deleteLibIncidentValidator;
        }
        public async Task<DeleteLibIncidentResponse> Handle(DeleteLibIncidentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteLibIncidentRequest));

            await _deleteLibIncidentValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _adminQuery.DeleteLibIncident(request);
            return result;
        }
    }
}
