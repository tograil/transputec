using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.NoAppUser
{
    public class NoAppUserHandler : IRequestHandler<NoAppUserRequest, NoAppUserResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly NoAppUserValidator _noAppUserValidator;
        private readonly ILogger<NoAppUserHandler> _logger;
        public NoAppUserHandler(IReportsQuery reportsQuery, NoAppUserValidator noAppUserValidator, ILogger<NoAppUserHandler> logger)
        {
            this._logger = logger;
            this._noAppUserValidator = noAppUserValidator;
            this._reportsQuery = reportsQuery;
        }
        public async Task<NoAppUserResponse> Handle(NoAppUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(NoAppUserRequest));
            await _noAppUserValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.NoAppUser(request);
            return result;
        }
    }
}
