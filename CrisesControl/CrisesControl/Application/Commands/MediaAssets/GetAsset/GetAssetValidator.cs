﻿using FluentValidation;

namespace CrisesControl.Api.Application.Commands.MediaAssets.GetAsset
{
    public class GetAssetValidator:AbstractValidator<GetAssetRequest>
    {
        public GetAssetValidator()
        {
            RuleFor(x => x.AssetId)
                .GreaterThan(0);
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);
        }
    }
}