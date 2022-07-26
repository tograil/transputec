using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetResponseCoordinates
{
    public class GetResponseCoordinatesHander : IRequestHandler<GetResponseCoordinatesRequest, GetResponseCoordinatesResponse>
    {
        private readonly ILogger<GetResponseCoordinatesHander> _logger;
        private readonly IReportsQuery _reportsQuery;
        private readonly GetResponseCoordinatesValidator _getResponseCoordinatesValidator;
        public GetResponseCoordinatesHander(ILogger<GetResponseCoordinatesHander> logger, IReportsQuery reportsQuery)
        {
            this._logger = logger;
            this._reportsQuery = reportsQuery;
        }
        public async Task<GetResponseCoordinatesResponse> Handle(GetResponseCoordinatesRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetResponseCoordinatesRequest));
            await _getResponseCoordinatesValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await  _reportsQuery.GetResponseCoordinates(request);
            return result;
        }
    }
}
