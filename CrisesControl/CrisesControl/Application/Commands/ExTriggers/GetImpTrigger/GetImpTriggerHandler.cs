using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.ExTriggers.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ExTriggers.GetImpTrigger
{
    public class GetImpTriggerHandler: IRequestHandler<GetImpTriggerRequest, GetImpTriggerResponse>
    {
        private readonly GetImpTriggerValidator _exTriggertValidator;
        private readonly IExTriggerQuery _exTriggerQuery;
        private readonly IExTriggerRepository _exTriggerRepository;

        public GetImpTriggerHandler(GetImpTriggerValidator exTriggertValidator, IExTriggerQuery exTriggerQuery, IExTriggerRepository exTriggerRepository)
        {
         this._exTriggerQuery = exTriggerQuery;
            this._exTriggertValidator=exTriggertValidator;
            this._exTriggerRepository=exTriggerRepository;
        }

        public async Task<GetImpTriggerResponse> Handle(GetImpTriggerRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetImpTriggerRequest));

            await _exTriggertValidator.ValidateAndThrowAsync(request, cancellationToken);

            var import = await _exTriggerQuery.GetImpTrigger(request);
            return import;
        }
    }
}
