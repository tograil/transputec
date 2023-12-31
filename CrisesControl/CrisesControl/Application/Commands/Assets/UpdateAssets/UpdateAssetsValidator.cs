﻿using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Assets.UpdateAssets
{
    public class UpdateAssetsValidator: AbstractValidator<UpdateAssetsRequest>
    {
        public UpdateAssetsValidator()
        {
            RuleFor(x => x.AssetId)
                .GreaterThan(0);
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);
        }
    }
}
