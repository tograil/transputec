using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.UploadSingleFile
{
    public class UploadSingleFileHandler : IRequestHandler<UploadSingleFileRequest, UploadSingleFileResponse>
    {
        private readonly UploadSingleFileValidator _importValidator;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public UploadSingleFileHandler(UploadSingleFileValidator importValidator, IImportRepository importRepository, IMapper mapper)
        {
            _importValidator = importValidator;
            _importRepository = importRepository;
            _mapper = mapper;
        }

        public async Task<UploadSingleFileResponse> Handle(UploadSingleFileRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UploadSingleFileRequest));
            _importRepository.UploadSingleFile();
            return null;
        }
    }
}
