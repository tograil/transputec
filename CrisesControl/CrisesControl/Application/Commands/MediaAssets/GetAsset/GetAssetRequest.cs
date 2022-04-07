﻿using MediatR;

namespace CrisesControl.Api.Application.Commands.MediaAssets.GetAsset
{
    public class GetAssetRequest: IRequest<GetAssetResponse>
    {
        public int CompanyId { get; set; }
        public int AssetId { get; set; }
    }
}
