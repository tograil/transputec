﻿using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.DeleteAssetLink
{
    public class DeleteAssetLinkRequest:IRequest<DeleteAssetLinkResponse>
    {
        public int AssetID { get; set; }
    }
}
