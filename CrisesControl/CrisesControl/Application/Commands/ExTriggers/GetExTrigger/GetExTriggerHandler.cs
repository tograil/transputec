using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.ExTriggers.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ExTriggers.GetExTrigger
{
    public class GetExTriggerHandler: IRequestHandler<GetExTriggerRequest, GetExTriggerResponse>
    {
        private readonly GetExTriggerValidator _exTriggertValidator;
        private readonly IExTriggerQuery _exTriggerQuery;
        private readonly IExTriggerRepository _exTriggerRepository;

        public GetExTriggerHandler(GetExTriggerValidator exTriggertValidator, IExTriggerQuery exTriggerQuery, IExTriggerRepository exTriggerRepository)
        {
            this._exTriggerQuery = exTriggerQuery;
            this._exTriggertValidator = exTriggertValidator;
            this._exTriggerRepository = exTriggerRepository;
        }
        public async Task<GetExTriggerResponse> Handle(GetExTriggerRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetExTriggerRequest));

            await _exTriggertValidator.ValidateAndThrowAsync(request, cancellationToken);

            var exT = await _exTriggerQuery.GetExTrigger(request);
            return exT;
        }
    }
}
