using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.GetValidationResult
{
    public class GetValidationResultHandler : IRequestHandler<GetValidationResultRequest, GetValidationResultResponse>
    {
        private readonly GetValidationResultValidator _importValidator;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public GetValidationResultHandler(GetValidationResultValidator importValidator, IImportRepository importRepository, IMapper mapper)
        {
            _importValidator = importValidator;
            _importRepository = importRepository;
            _mapper = mapper;
        }

        public async Task<GetValidationResultResponse> Handle(GetValidationResultRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetValidationResultRequest));

            ProcessImport value = _mapper.Map<GetValidationResultRequest, ProcessImport>(request);
            _importRepository.GetValidationResult(value, cancellationToken);
            return null;
        }
    }
}
