﻿using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Assets;
using CrisesControl.Core.Assets.Respositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.MediaAssets.CreateAsset
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

            Asset value = _mapper.Map<CreateAssetRequest, Asset>(request);

            if (CheckDuplicate(value))
            {
                var assetId = await _assetRepository.CreateAsset(value, cancellationToken);
                var result = new CreateAssetResponse();
                result.AssetId = assetId;
                return result;
            }
            return null;
        }

        private bool CheckDuplicate(Asset asset)
        {
            return _assetRepository.CheckDuplicate(asset);
        }
    }
}
