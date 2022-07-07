using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetEmailTemplate
{
    public class GetEmailTemplateHandler : IRequestHandler<GetEmailTemplateRequest, GetEmailTemplateResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<GetEmailTemplateHandler> _logger;
        private readonly GetEmailTemplateValidator _getEmailTemplateValidator;
        public GetEmailTemplateHandler(IAdminQuery adminQuery, ILogger<GetEmailTemplateHandler> logger, GetEmailTemplateValidator getEmailTemplateValidator)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
            this._getEmailTemplateValidator = getEmailTemplateValidator;
        }
        public async Task<GetEmailTemplateResponse> Handle(GetEmailTemplateRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetEmailTemplateRequest));

            await _getEmailTemplateValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _adminQuery.GetEmailTemplate(request);
            return result;
        }
    }
}
