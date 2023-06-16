using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetMessageAnslysisResponse
{
    public class GetMessageAnslysisResponseHandler : IRequestHandler<GetMessageAnslysisResponseRequest, GetMessageAnslysisResponseResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly GetMessageAnslysisResponseValidator _getMessageAnslysisResponseValidator;
        private readonly ILogger<GetMessageAnslysisResponseHandler> _logger;
        public GetMessageAnslysisResponseHandler(IReportsQuery reportsQuery, GetMessageAnslysisResponseValidator getMessageAnslysisResponseValidator, ILogger<GetMessageAnslysisResponseHandler> logger)
        {
            this._logger = logger;
            this._getMessageAnslysisResponseValidator = getMessageAnslysisResponseValidator;
            this._reportsQuery = reportsQuery;
        }
        public async Task<GetMessageAnslysisResponseResponse> Handle(GetMessageAnslysisResponseRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetMessageAnslysisResponseRequest));
            await _getMessageAnslysisResponseValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.GetMessageAnslysisResponse(request);
            return result;
        }
    }
}
