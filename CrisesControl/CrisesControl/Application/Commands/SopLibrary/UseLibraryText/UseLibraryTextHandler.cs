using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.SopLibrary.UseLibraryText
{
    public class UseLibraryTextHandler : IRequestHandler<UseLibraryTextRequest, UseLibraryTextResponse>
    {
        private readonly ISopLibraryQuery _sopLibraryQuery;
        private readonly ILogger<UseLibraryTextHandler> _logger;
        private readonly UseLibraryTextValidator _useLibraryTextValidator;
        public UseLibraryTextHandler(ISopLibraryQuery sopLibraryQuery, ILogger<UseLibraryTextHandler> logger, UseLibraryTextValidator useLibraryTextValidator)
        {
            this._logger = logger;
            this._sopLibraryQuery = sopLibraryQuery;
            this._useLibraryTextValidator = useLibraryTextValidator;
        }
        public async Task<UseLibraryTextResponse> Handle(UseLibraryTextRequest request, CancellationToken cancellationToken)
        {

            Guard.Against.Null(request, nameof(UseLibraryTextRequest));

            await _useLibraryTextValidator.ValidateAndThrowAsync(request, cancellationToken);

            var delete = await _sopLibraryQuery.UseLibraryText(request);
            return delete;
        }

        
    }
}
