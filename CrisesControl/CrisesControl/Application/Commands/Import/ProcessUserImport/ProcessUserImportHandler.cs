using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.ProcessUserImport
{
    public class ProcessUserImportHandler : IRequestHandler<ProcessUserImportRequest, ProcessUserImportResponse>
    {
        private readonly ProcessUserImportValidator _importValidator;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public ProcessUserImportHandler(ProcessUserImportValidator importValidator, IImportRepository groupRepository, IMapper mapper)
        {
            _importValidator = importValidator;
            _importRepository = groupRepository;
            _mapper = mapper;
        }

        public async Task<ProcessUserImportResponse> Handle(ProcessUserImportRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ProcessUserImportRequest));

            ProcessImport value = _mapper.Map<ProcessUserImportRequest, ProcessImport>(request);
            _importRepository.ProcessUserImport(value, cancellationToken);
            return null;
        }
    }
}
