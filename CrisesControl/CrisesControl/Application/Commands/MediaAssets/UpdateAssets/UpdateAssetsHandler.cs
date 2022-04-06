using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.AssetAggregate;
using CrisesControl.Core.AssetAggregate.Respositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.MediaAssets.UpdateAssets
{
    public class UpdateAssetsHandler:IRequestHandler<UpdateAssetsRequest, UpdateAssetsResponse>
    {
        private readonly UpdateAssetsValidator _assetValidator;
        private readonly IAssetRepository _assetRepository;
        private readonly IMapper _mapper;

        public UpdateAssetsHandler(UpdateAssetsValidator assetValidator, IAssetRepository assetRepository, IMapper mapper)
        {
            _assetValidator = assetValidator;
            _assetRepository = assetRepository;
            _mapper = mapper;
        }

        public async Task<UpdateAssetsResponse> Handle(UpdateAssetsRequest updateAssetsRequest, CancellationToken cancellationToken)
        {
            Guard.Against.Null(updateAssetsRequest, nameof(UpdateAssetsRequest));

            Assets value = _mapper.Map<UpdateAssetsRequest, Assets>(updateAssetsRequest);
            var assetId = await _assetRepository.UpdateAsset(value, cancellationToken);
            var result = new UpdateAssetsResponse();
            result.AssetId = assetId;
            return result;
        } 
    }
}
