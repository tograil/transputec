using AutoMapper;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.ApiUrlsById
{
    public class ApiUrlsByIdHandler : IRequestHandler<ApiUrlsByIdRequest, ApiUrlsByIdResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        private readonly ApiUrlsByIdValidator _apiUrlsByIdValidator;
        public ApiUrlsByIdHandler(ApiUrlsByIdValidator apiUrlsByIdValidator, IMapper mapper, IAdminRepository adminRepository)
        {
            this._adminRepository = adminRepository;
            this._apiUrlsByIdValidator = apiUrlsByIdValidator;
            this._mapper = mapper;
        }
        public async Task<ApiUrlsByIdResponse> Handle(ApiUrlsByIdRequest request, CancellationToken cancellationToken)
        {
            var api = await _adminRepository.GetApiUrlByIdAsync(request.ApiID);
            var result = _mapper.Map<CrisesControl.Core.Administrator.Api>(api);
            var response = new ApiUrlsByIdResponse();
            if (result != null)
            {
                response.Data = result;
            }
            else
            {
                response.Data = new Core.Administrator.Api();
            }
            return response;
        }
    }
}
