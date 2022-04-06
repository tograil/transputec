using AutoMapper;
using CrisesControl.Api.Application.Commands.MediaAssets.CreateAsset;
using CrisesControl.Api.Application.Commands.MediaAssets.GetAsset;
using CrisesControl.Api.Application.Commands.MediaAssets.UpdateAssets;
using CrisesControl.Core.AssetAggregate;

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
