using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.CreateActivationKey
{
    public class CreateActivationKeyHandler : IRequestHandler<CreateActivationKeyRequest, CreateActivationKeyResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public CreateActivationKeyHandler(IAdminRepository adminRepository, IMapper mapper, ICurrentUser currentUser)
        {
            this._adminRepository = adminRepository;
            this._mapper = mapper;
            this._currentUser = currentUser;
        }
        public async Task<CreateActivationKeyResponse> Handle(CreateActivationKeyRequest request, CancellationToken cancellationToken)
        {
            try
            {

                var Key = await _adminRepository.CreateActivationKey(request.CustomerId, _currentUser.UserId, request.SalesSource);
                var result = _mapper.Map<string>(Key);
                var response = new CreateActivationKeyResponse();
                if (result != null)
                {
                    response.Key = Key;
                    response.Message = "Key has been created";
                }
                else
                {
                    response.Key = null;
                    response.Message = "Key not created.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
