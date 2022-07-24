using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.GroupOnlyImport
{
    public class GroupOnlyImportHandler : IRequestHandler<GroupOnlyImportRequest, GroupOnlyImportResponse>
    {
        private readonly GroupOnlyImportValidator _importValidator;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public GroupOnlyImportHandler(GroupOnlyImportValidator importValidator, IImportRepository importRepository, IMapper mapper)
        {
            _importValidator = importValidator;
            _importRepository = importRepository;
            _mapper = mapper;
        }

        public async Task<GroupOnlyImportResponse> Handle(GroupOnlyImportRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GroupOnlyImportRequest));

            GroupOnlyImportModel value = _mapper.Map<GroupOnlyImportRequest, GroupOnlyImportModel>(request);
            _importRepository.GroupOnlyImport(value, cancellationToken);
            return null;
        }
    }
}
