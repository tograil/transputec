using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Models;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.SaveSocialIntegration
{
    public class SaveSocialIntegrationHandler : IRequestHandler<SaveSocialIntegrationRequest, SaveSocialIntegrationResponse>
    {
        private readonly ILogger<SaveSocialIntegrationHandler> _logger;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        public SaveSocialIntegrationHandler(ICompanyRepository companyRepository, ICurrentUser currentUser, ILogger<SaveSocialIntegrationHandler> logger, IMapper mapper)
        {
            this._logger = logger;
            this._companyRepository = companyRepository;
            this._currentUser = currentUser;
            this._mapper = mapper;
        }
        public async Task<SaveSocialIntegrationResponse> Handle(SaveSocialIntegrationRequest request, CancellationToken cancellationToken)
        {
         

            var social = await _companyRepository.SaveSocialIntegration(request.AccountName,request.AccountType,request.AuthSecret,request.AdnlKeyOne,request.AuthToken,request.AdnlKeyTwo, _currentUser.CompanyId,_currentUser.TimeZone,_currentUser.UserId);
            var result = _mapper.Map<bool>(social);
            var response = new SaveSocialIntegrationResponse();
            if (result)
            {

                response.Data = result;
                response.Message = "Data loaded";
            }
            else
            {
                response.Data = result;
                response.Message = "No data Found";
            }

            return response;
        }
    }
}
