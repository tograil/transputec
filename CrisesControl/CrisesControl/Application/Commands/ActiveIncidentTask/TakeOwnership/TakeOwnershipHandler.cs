using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.TakeOwnership
{
    public class TakeOwnershipHandler : IRequestHandler<TakeOwnershipRequest, TakeOwnershipResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<TakeOwnershipHandler> _logger;
        private readonly TakeOwnershipValidator _takeOwnershipValidator;
        public TakeOwnershipHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<TakeOwnershipHandler> logger, TakeOwnershipValidator takeOwnershipValidator)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
            this._takeOwnershipValidator = takeOwnershipValidator;
        }
        public async Task<TakeOwnershipResponse> Handle(TakeOwnershipRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TakeOwnershipRequest));
            await _takeOwnershipValidator.ValidateAndThrowAsync(request, cancellationToken); ;
            var result = await _activeIncidentQuery.TakeOwnership(request);
            return result;
        }
    }
}
