using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Repositories;
using CrisesControl.Core.Models;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.UserCompleteUpload
{
    public class UserCompleteUploadHandler : IRequestHandler<UserCompleteUploadRequest, UserCompleteUploadResponse>
    {
        private readonly UserCompleteUploadValidator _importValidator;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public UserCompleteUploadHandler(UserCompleteUploadValidator importValidator, IImportRepository importRepository, IMapper mapper, ICurrentUser currentUser)
        {
            _importValidator = importValidator;
            _importRepository = importRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<UserCompleteUploadResponse> Handle(UserCompleteUploadRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UserCompleteUploadRequest));

            //UserCompleteUploadModel value = _mapper.Map<UserCompleteUploadRequest, UserCompleteUploadModel>(request);
            var importUsers =await _importRepository.UserCompleteUpload(request.UserImportTotalId, _currentUser.CompanyId, cancellationToken, _currentUser.UserId, _currentUser.TimeZone, request.ImportAsActive);
            var result = _mapper.Map<CommonDTO>(importUsers);
            var resonse = new UserCompleteUploadResponse();
            resonse.Common = result;
            return resonse;
        }
    }
}
