using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetEmailFields
{
    public class GetEmailFieldsHandler : IRequestHandler<GetEmailFieldsRequest, GetEmailFieldsResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly GetEmailFieldsValidator _getEmailFieldsValidator;
        private ILogger<GetEmailFieldsHandler> _logger;
        public GetEmailFieldsHandler(IAdminQuery adminQuery, ILogger<GetEmailFieldsHandler> logger, GetEmailFieldsValidator getEmailFieldsValidator)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
            this._getEmailFieldsValidator = getEmailFieldsValidator;
        }
        public async Task<GetEmailFieldsResponse> Handle(GetEmailFieldsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetEmailFieldsRequest));

            await _getEmailFieldsValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _adminQuery.GetEmailFields(request);
            return result;
        }
    }
}
