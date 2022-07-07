using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.RestoreTemplate
{
    public class RestoreTemplateHandler : IRequestHandler<RestoreTemplateRequest, RestoreTemplateResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly RestoreTemplateValidator _restoreTemplateValidator;
        private ILogger<RestoreTemplateHandler> _logger;
        public RestoreTemplateHandler(IAdminQuery adminQuery, RestoreTemplateValidator restoreTemplateValidator, ILogger<RestoreTemplateHandler> logger)
        {
            this._logger = logger;
            this._adminQuery = adminQuery;
            this._restoreTemplateValidator = restoreTemplateValidator;
        }
        public async Task<RestoreTemplateResponse> Handle(RestoreTemplateRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(RestoreTemplateRequest));

            await _restoreTemplateValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _adminQuery.RestoreTemplate(request);
            return result;
        }
    }
}
