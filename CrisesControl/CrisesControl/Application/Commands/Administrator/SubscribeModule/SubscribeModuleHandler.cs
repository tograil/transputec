using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.SubscribeModule
{
    public class SubscribeModuleHandler : IRequestHandler<SubscribeModuleRequest, SubscribeModuleResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<SubscribeModuleHandler> _logger;
        private readonly SubscribeModuleValidator _subscribeModuleValidator;

        public SubscribeModuleHandler(IAdminQuery adminQuery, ILogger<SubscribeModuleHandler> logger, SubscribeModuleValidator subscribeModuleValidator)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
            this._subscribeModuleValidator = subscribeModuleValidator;

        }
        public async Task<SubscribeModuleResponse> Handle(SubscribeModuleRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SubscribeModuleRequest));

            await _subscribeModuleValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _adminQuery.SubscribeModule(request);
            return result;
        }
    }
}
