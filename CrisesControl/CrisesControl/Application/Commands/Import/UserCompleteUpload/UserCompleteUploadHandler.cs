using Ardalis.GuardClauses;
using AutoMapper;
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

        public UserCompleteUploadHandler(UserCompleteUploadValidator importValidator, IImportRepository importRepository, IMapper mapper)
        {
            _importValidator = importValidator;
            _importRepository = importRepository;
            _mapper = mapper;
        }

        public async Task<UserCompleteUploadResponse> Handle(UserCompleteUploadRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UserCompleteUploadRequest));

            UserCompleteUploadModel value = _mapper.Map<UserCompleteUploadRequest, UserCompleteUploadModel>(request);
            _importRepository.UserCompleteUpload(value, cancellationToken);
            return null;
        }
    }
}
