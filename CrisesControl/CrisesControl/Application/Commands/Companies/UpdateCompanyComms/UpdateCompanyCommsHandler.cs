using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyComms
{
    public class UpdateCompanyCommsHandler : IRequestHandler<UpdateCompanyCommsRequest, UpdateCompanyCommsResponse>
    {
        private readonly ILogger<UpdateCompanyCommsHandler> _logger;
        private readonly ICompanyQuery _companyQuery;
        private readonly UpdateCompanyCommsValidator _updateCompanyCommsValidator;
        public UpdateCompanyCommsHandler(ILogger<UpdateCompanyCommsHandler> logger, ICompanyQuery companyQuery, UpdateCompanyCommsValidator updateCompanyCommsValidator)
        {
            this._companyQuery = companyQuery;
            this._logger = logger;
            this._updateCompanyCommsValidator = updateCompanyCommsValidator;
        }
        public async Task<UpdateCompanyCommsResponse> Handle(UpdateCompanyCommsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateCompanyCommsRequest));

            await _updateCompanyCommsValidator.ValidateAndThrowAsync(request, cancellationToken);

            var companyInfo = await _companyQuery.UpdateCompanyComms(request);

            return companyInfo;
        }
    }
}
