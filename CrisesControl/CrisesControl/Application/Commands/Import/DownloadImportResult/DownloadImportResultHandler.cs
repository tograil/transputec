using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.DownloadImportResult
{
    public class DownloadImportResultHandler : IRequestHandler<DownloadImportResultRequest, DownloadImportResultResponse>
    {
        private readonly DownloadImportResultValidator _importValidator;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public DownloadImportResultHandler(DownloadImportResultValidator importValidator, IImportRepository importRepository, IMapper mapper)
        {
            _importValidator = importValidator;
            _importRepository = importRepository;
            _mapper = mapper;
        }

        public async Task<DownloadImportResultResponse> Handle(DownloadImportResultRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DownloadImportResultRequest));
            _importRepository.DownloadImportResult(request.CompanyId, request.SessionId, cancellationToken);
            return null;
        }

    }
}
