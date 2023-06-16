using AutoMapper;
using CrisesControl.Api.Application.Commands.Assets.CreateAsset;
using CrisesControl.Api.Application.Commands.Assets.GetAsset;
using CrisesControl.Api.Application.Commands.Assets.UpdateAssets;
using CrisesControl.Core.Assets;

namespace CrisesControl.Api.Application.Maps
{
    public class AssetProfile: Profile
    {
        public AssetProfile()
        {
            CreateMap<Assets, GetAssetResponse>();

            CreateMap<CreateAssetRequest, Assets>()
            .ForMember(x => x.CreatedOn, m => m.MapFrom(x => DateTimeOffset.Now))
            .ForMember(x => x.UpdatedOn, m => m.MapFrom(x => DateTimeOffset.Now));

            CreateMap<UpdateAssetsRequest, Assets>()
            .ForMember(x => x.UpdatedOn, m => m.MapFrom(x => DateTimeOffset.Now));
        }
    }
}
