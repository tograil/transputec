using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Commands.Companies.GetStarted;
using CrisesControl.Core.Companies.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.GetStarted
{
    public class GetStartedResponseHandler:IRequestHandler<GetStartedRequest, GetStartedResponse>
    {
        private readonly GetStartedValidator _getStartedValidator;
        private readonly ICompanyRepository _companyQuery;
        public GetStartedResponseHandler(GetStartedValidator getStartedValidator, ICompanyRepository companyQuery)
        {
            this._companyQuery = companyQuery;
            this._getStartedValidator = getStartedValidator;
        }

        public async Task<GetStartedResponse> Handle(GetStartedRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetStartedRequest));

            await _getStartedValidator.ValidateAndThrowAsync(request, cancellationToken);

            var companyInfo = await _companyQuery.GetStarted(request.CompanyId);
            var response = new GetStartedResponse();
            if (companyInfo!=null) { 
           
            response.Data = companyInfo;
            response.Message = "Data loaded";
            }
            else
            {
                response.Data = companyInfo;
                response.Message = "No data Found";
            }
                    
            return response;
        }
    }
}
