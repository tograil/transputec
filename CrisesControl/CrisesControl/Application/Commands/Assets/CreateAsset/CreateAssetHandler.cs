using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Assets.Respositories;
using CrisesControl.Core.Models;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.CreateAsset
{
    public class CreateAssetHandler : IRequestHandler<CreateAssetRequest, CreateAssetResponse>
    {
        private readonly CreateAssetValidator _assetValidator;
        private readonly IAssetQuery _assetQuery;
        private readonly ILogger<CreateAssetHandler> _logger;

        public CreateAssetHandler(CreateAssetValidator assetValidator, IAssetQuery assetRepository, ILogger<CreateAssetHandler> logger)
        {
            _assetValidator = assetValidator;
            _assetQuery = assetRepository;
            _logger = logger;

        }

        public async Task<CreateAssetResponse> Handle(CreateAssetRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateAssetRequest));

            await _assetValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _assetQuery.CreateAsset(request, cancellationToken);
            return result;
        }

    }
}
