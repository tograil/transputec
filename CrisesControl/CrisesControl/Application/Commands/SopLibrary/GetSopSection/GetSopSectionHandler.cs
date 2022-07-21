using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.SopLibrary.GetSopSection
{
    public class GetSopSectionHandler:IRequestHandler<GetSopSectionRequest, GetSopSectionResponse>
    {
        private readonly ISopLibraryQuery _sopLibraryQuery;
        private readonly ILogger<GetSopSectionHandler> _logger;
        private readonly GetSopSectionValidator _getSopSectionValidatorValidator;
        public GetSopSectionHandler(ISopLibraryQuery sopLibraryQuery, ILogger<GetSopSectionHandler> logger, GetSopSectionValidator getSopSectionValidatorValidator)
        {
            this._logger = logger;
            this._sopLibraryQuery = sopLibraryQuery;
            this._getSopSectionValidatorValidator = getSopSectionValidatorValidator;
        }

        public async Task<GetSopSectionResponse> Handle(GetSopSectionRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetSopSectionRequest));

            await _getSopSectionValidatorValidator.ValidateAndThrowAsync(request, cancellationToken);

            var getSection = await _sopLibraryQuery.GetSopSection(request);
            return getSection;
        }
    }
}
