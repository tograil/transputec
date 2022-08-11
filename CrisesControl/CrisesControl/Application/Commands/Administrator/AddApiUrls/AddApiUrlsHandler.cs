using AutoMapper;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.AddApiUrls
{
    public class AddApiUrlsHandler : IRequestHandler<AddApiUrlsRequest, AddApiUrlsResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
       
        public AddApiUrlsHandler( IMapper mapper, IAdminRepository adminRepository)
        {
            this._adminRepository = adminRepository;
            this._mapper = mapper;
        }
        public async Task<AddApiUrlsResponse> Handle(AddApiUrlsRequest request, CancellationToken cancellationToken)
        {
            var api = await _adminRepository.AddApiUrlAsync(request.ApiUrl, request.ApiHost, request.IsCurrent, request.Status, request.Version, request.AppVersion, request.ApiMode, request.Platform);
            var result = _mapper.Map<CrisesControl.Core.Administrator.Api>(api);
            var response = new AddApiUrlsResponse();
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
