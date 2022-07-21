using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Repositories;
using CrisesControl.Core.Models;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.DepartmentOnlyUpload
{
    public class DepartmentOnlyUploadHandler : IRequestHandler<DepartmentOnlyUploadRequest, DepartmentOnlyUploadResponse>
    {
        private readonly DepartmentOnlyUploadValidator _importValidator;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public DepartmentOnlyUploadHandler(DepartmentOnlyUploadValidator importValidator, IImportRepository importRepository, IMapper mapper)
        {
            _importValidator = importValidator;
            _importRepository = importRepository;
            _mapper = mapper;
        }

        public async Task<DepartmentOnlyUploadResponse> Handle(DepartmentOnlyUploadRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DepartmentOnlyUploadRequest));

            GroupOnlyUploadModel value = _mapper.Map<DepartmentOnlyUploadRequest, GroupOnlyUploadModel>(request);
            _importRepository.DepartmentOnlyUpload(value, cancellationToken);
            return null;
        }
    }
}
