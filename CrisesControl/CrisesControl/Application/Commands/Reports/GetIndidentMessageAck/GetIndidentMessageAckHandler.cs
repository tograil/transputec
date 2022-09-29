using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck
{
    public class GetIndidentMessageAckHandler:IRequestHandler<GetIndidentMessageAckRequest, GetIndidentMessageAckResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly GetIndidentMessageAckValidator _getIndidentMessageAckValidator;

        public GetIndidentMessageAckHandler(IReportsQuery reportsQuery, GetIndidentMessageAckValidator getIndidentMessageAckValidator)
        {
            _reportsQuery = reportsQuery;
            _getIndidentMessageAckValidator = getIndidentMessageAckValidator;
        }

        public async Task<GetIndidentMessageAckResponse> Handle(GetIndidentMessageAckRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetIndidentMessageAckRequest));
            await _getIndidentMessageAckValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.GetIndidentMessageAck(request, null);
            return null;
        }
    }
}
