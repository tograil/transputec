using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.LocationOnlyImport
{
    public class LocationOnlyImportHandler : IRequestHandler<LocationOnlyImportRequest, LocationOnlyImportResponse>
    {
        private readonly LocationOnlyImportValidator _importValidator;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public LocationOnlyImportHandler(LocationOnlyImportValidator importValidator, IImportRepository importRepository, IMapper mapper)
        {
            _importValidator = importValidator;
            _importRepository = importRepository;
            _mapper = mapper;
        }

        public async Task<LocationOnlyImportResponse> Handle(LocationOnlyImportRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(LocationOnlyImportRequest));

            LocationOnlyImportModel value = _mapper.Map<LocationOnlyImportRequest, LocationOnlyImportModel>(request);
            _importRepository.LocationOnlyImport(value, cancellationToken);
            return null;
        }
    }
}
