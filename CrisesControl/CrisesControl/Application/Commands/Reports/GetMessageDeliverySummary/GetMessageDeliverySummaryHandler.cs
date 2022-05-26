
using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Reports.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetMessageDeliverySummary
{
    public class GetMessageDeliverySummaryHandler : IRequestHandler<GetMessageDeliverySummaryRequest, GetMessageDeliverySummaryResponse>
    {
        private readonly GetMessageDeliverySummaryValidator _getMessageDeliverySummaryValidator;
        private readonly ILogger<GetMessageDeliverySummaryHandler> _logger;
        private readonly IReportsQuery _reportsQuery;
        public GetMessageDeliverySummaryHandler(GetMessageDeliverySummaryValidator getMessageDeliverySummaryValidator, ILogger<GetMessageDeliverySummaryHandler> logger, IReportsQuery reportsQuery)
        {
            this._getMessageDeliverySummaryValidator = getMessageDeliverySummaryValidator;
            this._reportsQuery=reportsQuery;
            this._logger = logger;
        }
        public async Task<GetMessageDeliverySummaryResponse> Handle(GetMessageDeliverySummaryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                Guard.Against.Null(request, nameof(GetMessageDeliverySummaryRequest));
                await _getMessageDeliverySummaryValidator.ValidateAndThrowAsync(request, cancellationToken);

                var result = await _reportsQuery.GetMessageDeliverySummary(request);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                           ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return new GetMessageDeliverySummaryResponse { };
            }
        }
    }
}
