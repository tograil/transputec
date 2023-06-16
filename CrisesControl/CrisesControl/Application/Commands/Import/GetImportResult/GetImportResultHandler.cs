using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.GetImportResult
{
    public class GetImportResultHandler : IRequestHandler<GetImportResultRequest, GetImportResultResponse>
    {
        private readonly GetImportResultValidator _importValidator;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public GetImportResultHandler(GetImportResultValidator importValidator, IImportRepository importRepository, IMapper mapper)
        {
            _importValidator = importValidator;
            _importRepository = importRepository;
            _mapper = mapper;
        }

        public async Task<GetImportResultResponse> Handle(GetImportResultRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetImportResultRequest));

            ProcessImport value = _mapper.Map<GetImportResultRequest, ProcessImport>(request);
            _importRepository.GetImportResult(value, cancellationToken);
            return null;
        }

    }
}
