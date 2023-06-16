using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.LocationOnlyUpload
{
    public class LocationOnlyUploadHandler : IRequestHandler<LocationOnlyUploadRequest, LocationOnlyUploadResponse>
    {
        private readonly LocationOnlyUploadValidator _importValidator;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public LocationOnlyUploadHandler(LocationOnlyUploadValidator importValidator, IImportRepository importRepository, IMapper mapper)
        {
            _importValidator = importValidator;
            _importRepository = importRepository;
            _mapper = mapper;
        }

        public async Task<LocationOnlyUploadResponse> Handle(LocationOnlyUploadRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(LocationOnlyUploadRequest));

            LocationOnlyUploadModel value = _mapper.Map<LocationOnlyUploadRequest, LocationOnlyUploadModel>(request);
            _importRepository.LocationOnlyUpload(value, cancellationToken);
            return null;
        }
    }
}
