using AutoMapper;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.ApiUrls
{
    public class ApiUrlsHandler : IRequestHandler<ApiUrlsRequest, ApiUrlsResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        public ApiUrlsHandler(IAdminRepository adminRepository, IMapper mapper)
        {
            this._adminRepository = adminRepository;
            this._mapper = mapper;
        }
        public async Task<ApiUrlsResponse> Handle(ApiUrlsRequest request, CancellationToken cancellationToken)
        {
            var UrlApis = await _adminRepository.GetApiUrlsAsync();
            var result = _mapper.Map<List<CrisesControl.Core.Administrator.Api>>(UrlApis);
            var response = new ApiUrlsResponse();
            if (result != null)
            {
                response.Data = result;
            }
            else
            {
                response.Data = result;
            }
            return response;
        }
    }
}
