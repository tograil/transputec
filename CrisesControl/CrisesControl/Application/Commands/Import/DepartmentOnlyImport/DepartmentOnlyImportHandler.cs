using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.DepartmentOnlyImport
{
    public class DepartmentOnlyImportHandler : IRequestHandler<DepartmentOnlyImportRequest, DepartmentOnlyImportResponse>
    {
        private readonly DepartmentOnlyImportValidator _importValidator;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public DepartmentOnlyImportHandler(DepartmentOnlyImportValidator importValidator, IImportRepository importRepository, IMapper mapper)
        {
            _importValidator = importValidator;
            _importRepository = importRepository;
            _mapper = mapper;
        }

        public async Task<DepartmentOnlyImportResponse> Handle(DepartmentOnlyImportRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DepartmentOnlyImportRequest));

            GroupOnlyImportModel value = _mapper.Map<DepartmentOnlyImportRequest, GroupOnlyImportModel>(request);
            _importRepository.DepartmentOnlyImport(value, cancellationToken);
            return null;
        }
    }
}
