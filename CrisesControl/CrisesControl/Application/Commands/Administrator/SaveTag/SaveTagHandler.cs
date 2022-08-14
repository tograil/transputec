using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.SaveTag
{
    public class SaveTagHandler : IRequestHandler<SaveTagRequest, SaveTagResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<SaveTagHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public SaveTagHandler(IAdminRepository adminRepository, ICurrentUser currentUser, ILogger<SaveTagHandler> logger, IMapper mapper)
        {
            this._adminRepository = adminRepository;
            this._logger = logger;
            this._mapper = mapper;
        }
        public async Task<SaveTagResponse> Handle(SaveTagRequest request, CancellationToken cancellationToken)
        {
            var groups = await _adminRepository.SaveTag(request.TagID, request.TagCategoryID, request.TagName, request.SearchTerms, _currentUser.UserId, _currentUser.TimeZone);
            var result = _mapper.Map<int>(groups);
            var response = new SaveTagResponse();
            if (result > 0)
            {
                response.TagId = result;
            }
            else
            {
                response.TagId = 0;
            }
            return response;
        }
    }
}
