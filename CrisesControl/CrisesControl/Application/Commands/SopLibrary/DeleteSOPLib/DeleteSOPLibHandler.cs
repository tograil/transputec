using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.SopLibrary.DeleteSOPLib
{
    public class DeleteSOPLibHandler : IRequestHandler<DeleteSOPLibRequest, DeleteSOPLibResponse>
    {
        private readonly ISopLibraryQuery _sopLibraryQuery;
        private readonly ILogger<DeleteSOPLibHandler> _logger;
        private readonly DeleteSOPLibValidator _deleteSOPLibValidator;
        public DeleteSOPLibHandler(ISopLibraryQuery sopLibraryQuery, ILogger<DeleteSOPLibHandler> logger, DeleteSOPLibValidator deleteSOPLibValidator)
        {
            this._logger = logger;
            this._sopLibraryQuery = sopLibraryQuery;
            this._deleteSOPLibValidator = deleteSOPLibValidator;
        }
        public async Task<DeleteSOPLibResponse> Handle(DeleteSOPLibRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteSOPLibRequest));

            await _deleteSOPLibValidator.ValidateAndThrowAsync(request, cancellationToken);

            var delete = await _sopLibraryQuery.DeleteSOPLib(request);
            return delete;
        }
    }
}
