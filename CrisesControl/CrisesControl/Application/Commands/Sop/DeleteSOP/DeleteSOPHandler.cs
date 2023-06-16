using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.DeleteSOP
{
    public class DeleteSOPHandler : IRequestHandler<DeleteSOPRequest, DeleteSOPResponse>
    {
        private ISopQuery _sopQuery;
        private readonly ILogger<DeleteSOPHandler> _logger;
        private readonly DeleteSOPValidator _deleteSOPValidator;
        public DeleteSOPHandler(ISopQuery sopQuery, ILogger<DeleteSOPHandler> logger, DeleteSOPValidator deleteSOPValidator)
        {
            _sopQuery = sopQuery;
            _logger = logger;
            _deleteSOPValidator = deleteSOPValidator;
        }
        public async Task<DeleteSOPResponse> Handle(DeleteSOPRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteSOPRequest));

            await _deleteSOPValidator.ValidateAndThrowAsync(request, cancellationToken);

            var securities = await _sopQuery.DeleteSOP(request);
            return securities;
        }
    }
}
