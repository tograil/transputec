using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetTrackingData
{
    public class GetTrackingDataHandler : IRequestHandler<GetTrackingDataRequest, GetTrackingDataResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly ILogger<GetTrackingDataHandler> _logger;
        private readonly GetTrackingDataValidator _getTrackingDataValidator;
        public GetTrackingDataHandler(ILogger<GetTrackingDataHandler> logger, IReportsQuery reportsQuery, GetTrackingDataValidator getTrackingDataValidator)
        {
            this._reportsQuery = reportsQuery;
            this._logger = logger;
            this._getTrackingDataValidator = getTrackingDataValidator;
        }
        public async Task<GetTrackingDataResponse> Handle(GetTrackingDataRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetTrackingDataRequest));
            await _getTrackingDataValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.GetTrackingData(request);
            return result;
        }
    }
}
