using AutoMapper;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.UpdateApiUrls
{
    public class UpdateApiUrlsHandler:IRequestHandler<UpdateApiUrlsRequest, UpdateApiUrlsResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;

        public UpdateApiUrlsHandler(IMapper mapper, IAdminRepository adminRepository)
        {
            this._adminRepository = adminRepository;
            this._mapper = mapper;
        }
        public async Task<UpdateApiUrlsResponse> Handle(UpdateApiUrlsRequest request, CancellationToken cancellationToken)
        {
            var api = await _adminRepository.UpdateApiUrlAsync(request.Api);
            var result = _mapper.Map<CrisesControl.Core.Administrator.Api>(api);
            var response = new UpdateApiUrlsResponse();
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
