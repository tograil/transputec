using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.SegregationLinks
{
    public class SegregationLinksHandler : IRequestHandler<SegregationLinksRequest, SegregationLinksResponse>
    {
        private readonly ILogger<SegregationLinksHandler> _logger;
        private readonly SegregationLinksValidator _segregationLinksValidator;
        private readonly IGroupQuery _groupQuery;
        

        public SegregationLinksHandler(ILogger<SegregationLinksHandler> logger, SegregationLinksValidator segregationLinksValidator, IGroupQuery groupQuery)
        {
            this._segregationLinksValidator = segregationLinksValidator;
            this._logger = logger;
            this._groupQuery = groupQuery;
        }
        public async Task<SegregationLinksResponse> Handle(SegregationLinksRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SegregationLinksRequest));
            await _segregationLinksValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _groupQuery.SegregationLinks(request);
            return result;
        }
    }
}
