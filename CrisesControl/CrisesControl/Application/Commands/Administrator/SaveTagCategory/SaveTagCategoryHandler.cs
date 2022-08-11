using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.SaveTagCategory
{
    public class SaveTagCategoryHandler : IRequestHandler<SaveTagCategoryRequest, SaveTagCategoryResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<SaveTagCategoryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public SaveTagCategoryHandler(IAdminRepository adminRepository, ICurrentUser currentUser, ILogger<SaveTagCategoryHandler> logger, IMapper mapper)
        {
            this._adminRepository = adminRepository;
            this._logger = logger;
            this._mapper = mapper;
        }
        public async Task<SaveTagCategoryResponse> Handle(SaveTagCategoryRequest request, CancellationToken cancellationToken)
        {
            var groups = await _adminRepository.SaveTagCategory(request.TagCategoryID, request.TagCategoryName, request.TagCategorySearchTerms, _currentUser.UserId, _currentUser.TimeZone);
            var result = _mapper.Map<int>(groups);
            var response = new SaveTagCategoryResponse();
            if (result >0)
            {
                response.TagCategoryId = result;
            }
            else
            {
                response.TagCategoryId = 0;
            }
            return response;
        }
    }
}
