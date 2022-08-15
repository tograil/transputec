using AutoMapper;
using CrisesControl.Core.Administrator.Repositories;
using CrisesControl.Core.Models;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetTag
{
    public class GetTagHandler : IRequestHandler<GetTagRequest, GetTagResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        public GetTagHandler(IAdminRepository adminRepository, IMapper mapper)
        {
            this._adminRepository = adminRepository;
            this._mapper = mapper;
        }
        public async Task<GetTagResponse> Handle(GetTagRequest request, CancellationToken cancellationToken)
        {
            var tag = await _adminRepository.GetTag(request.TagId);
            var result = _mapper.Map<Tag>(tag);
            var response = new GetTagResponse();
            if (result != null)
            {
                response.Tag = result;
            }
            else
            {
                response.Tag = null;
            }
            return response;
        }
    }
}
