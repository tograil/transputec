using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentMessageAck;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck
{
    public class GetIndidentMessageAckHandler:IRequestHandler<GetIncidentMessageAckRequest, GetIncidentMessageAckResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly GetIncidentMessageAckValidator _getIndidentMessageAckValidator;

        public GetIndidentMessageAckHandler(IReportsQuery reportsQuery, GetIncidentMessageAckValidator getIndidentMessageAckValidator)
        {
            _reportsQuery = reportsQuery;
            _getIndidentMessageAckValidator = getIndidentMessageAckValidator;
        }

        public async Task<GetIncidentMessageAckResponse> Handle(GetIncidentMessageAckRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetIncidentMessageAckRequest));
            await _getIndidentMessageAckValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.GetIncidentMessageAck(request);
            return null;
        }
    }
}
