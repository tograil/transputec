using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.DeleteLocation
{
    public class DeleteLocationHandler : IRequestHandler<DeleteLocationRequest, DeleteLocationResponse>
    {
        private readonly ILocationQuery _locationQuery;
        private readonly DeleteLocationValidator _deleteLocationValidator;
        public DeleteLocationHandler(ILocationQuery locationQuery, DeleteLocationValidator deleteLocationValidator)
        {
            this._locationQuery = locationQuery;
            this._deleteLocationValidator = deleteLocationValidator;
        }
        public async Task<DeleteLocationResponse> Handle(DeleteLocationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteLocationRequest));
            await _deleteLocationValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _locationQuery.DeleteLocation(request);
            return result;
        }
    }
}
