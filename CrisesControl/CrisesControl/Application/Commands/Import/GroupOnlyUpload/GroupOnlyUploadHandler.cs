using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.GroupOnlyUpload
{
    public class GroupOnlyUploadHandler : IRequestHandler<GroupOnlyUploadRequest, GroupOnlyUploadResponse>
    {
        private readonly GroupOnlyUploadValidator _importValidator;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public GroupOnlyUploadHandler(GroupOnlyUploadValidator importValidator, IImportRepository importRepository, IMapper mapper)
        {
            _importValidator = importValidator;
            _importRepository = importRepository;
            _mapper = mapper;
        }

        public async Task<GroupOnlyUploadResponse> Handle(GroupOnlyUploadRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GroupOnlyUploadRequest));

            GroupOnlyUploadModel value = _mapper.Map<GroupOnlyUploadRequest, GroupOnlyUploadModel>(request);
            _importRepository.GroupOnlyUpload(value, cancellationToken);
            return null;
        }
    }
}
