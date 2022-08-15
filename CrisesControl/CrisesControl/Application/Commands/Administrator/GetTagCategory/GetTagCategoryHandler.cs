using AutoMapper;
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetTagCategory
{
    public class GetTagCategoryHandler : IRequestHandler<GetTagCategoryRequest, GetTagCategoryResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        public GetTagCategoryHandler(IAdminRepository adminRepository, IMapper mapper)
        {
            this._adminRepository = adminRepository;
            this._mapper = mapper;
        }
        public async Task<GetTagCategoryResponse> Handle(GetTagCategoryRequest request, CancellationToken cancellationToken)
        {
            var groups = await _adminRepository.GetTagCategory(request.TagCategoryID);
            var result = _mapper.Map<CategoryTag>(groups);
            var response = new GetTagCategoryResponse();
            if (result!= null)
            {
                response.CategoryTag = result;
            }
            else
            {
                response.CategoryTag =result;
            }
            return response;
        }
    }
}
