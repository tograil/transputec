using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.ExTriggers.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ExTriggers.GetAllExTrigger
{
    public class GetAllExTriggerHandler : IRequestHandler<GetAllExTriggerRequest, GetAllExTriggerResponse>
    {
        private readonly GetAllExTriggerValidator _exTriggertValidator;
        private readonly IExTriggerQuery _exTriggerQuery;
        private readonly IExTriggerRepository _exTriggerRepository;

        public GetAllExTriggerHandler(GetAllExTriggerValidator exTriggertValidator, IExTriggerQuery exTriggerQuery, IExTriggerRepository exTriggerRepository)
        {
            this._exTriggerQuery = exTriggerQuery;
            this._exTriggertValidator = exTriggertValidator;
            this._exTriggerRepository=exTriggerRepository;

        }
        public async Task<GetAllExTriggerResponse> Handle(GetAllExTriggerRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAllExTriggerRequest));

            await _exTriggertValidator.ValidateAndThrowAsync(request, cancellationToken);

            var exT = await _exTriggerQuery.GetAllExTrigger(request);

            return exT;
        }
    }
}
