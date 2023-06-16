using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Assets;
using CrisesControl.Core.Assets.Respositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.UpdateAssets
{
    public class UpdateAssetsHandler:IRequestHandler<UpdateAssetsRequest, UpdateAssetsResponse>
    {
        private readonly UpdateAssetsValidator _assetValidator;
        private readonly IAssetQuery _assetQuery;
        private readonly ILogger<UpdateAssetsHandler> _logger;

        public UpdateAssetsHandler(UpdateAssetsValidator assetValidator, IAssetQuery assetRepository, ILogger<UpdateAssetsHandler> logger)
        {
            _assetValidator = assetValidator;
            _assetQuery = assetRepository;
            _logger = logger;
           
        }

        public async Task<UpdateAssetsResponse> Handle(UpdateAssetsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateAssetsRequest));
            await _assetValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _assetQuery.UpdateAssets(request, cancellationToken); 
            return result;
        } 
    }
}
