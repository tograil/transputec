using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.QueueImportJob
{
    public class QueueImportJobHandler : IRequestHandler<QueueImportJobRequest, QueueImportJobResponse>
    {
        private readonly QueueImportJobValidator _importValidator;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public QueueImportJobHandler(QueueImportJobValidator importValidator, IImportRepository importRepository, IMapper mapper)
        {
            _importValidator = importValidator;
            _importRepository = importRepository;
            _mapper = mapper;
        }

        public async Task<QueueImportJobResponse> Handle(QueueImportJobRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(QueueImportJobRequest));

            QueueImport value = _mapper.Map<QueueImportJobRequest, QueueImport>(request);
            _importRepository.QueueImportJob(value, cancellationToken);
            return null;
        }
    }
}
