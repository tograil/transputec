using AutoMapper;
using CrisesControl.Api.Application.Commands.MediaAssets.GetAsset;
using CrisesControl.Core.AssetAggregate;

namespace CrisesControl.Api.Application.Maps
{
    public class AssetProfile: Profile
    {
        public AssetProfile()
        {
            CreateMap<Assets, GetAssetResponse>();
        }
    }
}
