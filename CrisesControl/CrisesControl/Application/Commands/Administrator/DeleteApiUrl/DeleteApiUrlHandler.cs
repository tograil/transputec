using AutoMapper;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.DeleteApiUrl
{
    public class DeleteApiUrlHandler:IRequestHandler<DeleteApiUrlRequest, DeleteApiUrlResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        private readonly DeleteApiUrlValidator _deleteApiUrlValidator;
        public DeleteApiUrlHandler(DeleteApiUrlValidator deleteApiUrlValidator, IMapper mapper, IAdminRepository adminRepository)
        {
            this._adminRepository = adminRepository;
            this._deleteApiUrlValidator = deleteApiUrlValidator;
            this._mapper = mapper;
        }

        public async Task<DeleteApiUrlResponse> Handle(DeleteApiUrlRequest request, CancellationToken cancellationToken)
        {
            var api = await _adminRepository.DeleteApiUrlAsync(request.ApiID);
            var result = _mapper.Map<CrisesControl.Core.Administrator.Api>(api);
            var response = new DeleteApiUrlResponse();
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
