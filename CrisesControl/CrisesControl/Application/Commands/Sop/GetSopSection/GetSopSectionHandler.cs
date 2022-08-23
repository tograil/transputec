using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.GetSopSection
{
    public class GetSopSectionHandler:IRequestHandler<GetSopSectionRequest, GetSopSectionResponse>
    {
        private ISopQuery _sopQuery;
        private readonly ILogger<GetSopSectionHandler> _logger;
        private readonly GetSopSectionValidator _getSopSectionValidator;
        public GetSopSectionHandler(ISopQuery sopQuery, ILogger<GetSopSectionHandler> logger, GetSopSectionValidator getSopSectionValidator)
        {
            _sopQuery = sopQuery;
            _logger = logger;
            _getSopSectionValidator = getSopSectionValidator;
        }

        public async Task<GetSopSectionResponse> Handle(GetSopSectionRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetSopSectionRequest));

            await _getSopSectionValidator.ValidateAndThrowAsync(request, cancellationToken);

            var securities = await _sopQuery.GetSopSection(request);
            return securities;
        }
    }
}
