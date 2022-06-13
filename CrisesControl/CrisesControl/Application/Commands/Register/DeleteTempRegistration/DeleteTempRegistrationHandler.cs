using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Register.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.DeleteTempRegistration
{
    public class DeleteTempRegistrationHandler : IRequestHandler<DeleteTempRegistrationRequest, DeleteTempRegistrationResponse>
    {
        public readonly IRegisterQuery _registerQuery;
        private readonly ILogger<DeleteTempRegistrationHandler> _logger;
        private readonly DeleteTempRegistrationValidator _deleteTampValidator;
        public DeleteTempRegistrationHandler(ILogger<DeleteTempRegistrationHandler> logger, IRegisterQuery registerQuery)
        {
            this._registerQuery = registerQuery;
            this._logger = logger;
        }
        public async Task<DeleteTempRegistrationResponse> Handle(DeleteTempRegistrationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteTempRegistrationRequest));

            await _deleteTampValidator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await _registerQuery.DeleteTempRegistration(request);
            return response;
        }
    }
}
