using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Register;
using CrisesControl.Core.Register.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.UpdateCompanyStatus
{
    public class UpdateCompanyStatusHandler : IRequestHandler<UpdateCompanyStatusRequest, UpdateCompanyStatusResponse>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<UpdateCompanyStatusHandler> _logger;
        private readonly ICurrentUser _currentUser;  
        public UpdateCompanyStatusHandler(IRegisterRepository registerRepository, ICurrentUser currentUser, ILogger<UpdateCompanyStatusHandler> logger)
        {
            _registerRepository = registerRepository;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<UpdateCompanyStatusResponse> Handle(UpdateCompanyStatusRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateCompanyStatusRequest));
            var response = new UpdateCompanyStatusResponse();
            var mappedRequest = new ViewCompanyModel();  //_mapper.Map<ViewCompanyModel>(request);
            mappedRequest.CompanyProfile = request.CompanyProfile;
            mappedRequest.Status = request.Status;
            mappedRequest.CurrentUserId = _currentUser.UserId;
            mappedRequest.CompanyId = _currentUser.CompanyId;
            response.Result = await _registerRepository.UpdateCompanyStatus(mappedRequest);
            return response;
        }
    }
}
