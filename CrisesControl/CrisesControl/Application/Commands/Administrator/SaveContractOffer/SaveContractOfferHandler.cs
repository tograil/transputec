using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.SaveContractOffer
{
    public class SaveContractOfferHandler:IRequestHandler<SaveContractOfferRequest, SaveContractOfferResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<SaveContractOfferHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public SaveContractOfferHandler(IAdminRepository adminRepository, ICurrentUser currentUser, ILogger<SaveContractOfferHandler> logger, IMapper mapper)
        {
            this._adminRepository = adminRepository;
            this._logger = logger;
            this._mapper = mapper;
        }

        public async Task<SaveContractOfferResponse> Handle(SaveContractOfferRequest request, CancellationToken cancellationToken)
        {
            var groups = await _adminRepository.SaveContractOffer(request.IP, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
            var result = _mapper.Map<bool>(groups);
            var response = new SaveContractOfferResponse();
            if (result)
            {
                response.result = true;
                response.Message = "Contract has been Saved.";
            }
            else
            {
                response.result = true;
                response.Message = "Contract has not created";
            }
            return response;
        }
    }
}
