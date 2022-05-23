using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Assets.Respositories;
using CrisesControl.Core.Models;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.CreateAsset
{
    public class CreateAssetHandler : IRequestHandler<CreateAssetRequest, CreateAssetResponse>
    {
        private readonly CreateAssetValidator _assetValidator;
        private readonly IAssetRepository _assetRepository;
        private readonly IMapper _mapper;

        public CreateAssetHandler(CreateAssetValidator assetValidator, IAssetRepository assetRepository, IMapper mapper)
        {
            _assetValidator = assetValidator;
            _assetRepository = assetRepository;
            _mapper = mapper;

        }

        public async Task<CreateAssetResponse> Handle(CreateAssetRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateAssetRequest));

            Core.Assets.Assets value = _mapper.Map<CreateAssetRequest, Core.Assets.Assets>(request);

            if (!CheckDuplicate(value))
            {
                var assetId = await _assetRepository.CreateAsset(value, cancellationToken);
                var result = new CreateAssetResponse();
                result.AssetId = assetId;
                return result;
            }
            return null;
        }

        private bool CheckDuplicate(Core.Assets.Assets asset)
        {
            return _assetRepository.CheckDuplicate(asset);
        }
    }
}
