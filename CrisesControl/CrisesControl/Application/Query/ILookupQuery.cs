using CrisesControl.Api.Application.Commands.Lookup.AssetTypes;
using CrisesControl.Api.Application.Commands.Lookup.GetAllTmpDept;
using CrisesControl.Api.Application.Commands.Lookup.GetAllTmpLoc;
using CrisesControl.Api.Application.Commands.Lookup.GetAllTmpUser;
using CrisesControl.Api.Application.Commands.Lookup.GetCountry;
using CrisesControl.Api.Application.Commands.Lookup.GetIcons;
using CrisesControl.Api.Application.Commands.Lookup.GetImportTemplates;
using CrisesControl.Api.Application.Commands.Lookup.GetTempDept;
using CrisesControl.Api.Application.Commands.Lookup.GetTempLoc;
using CrisesControl.Api.Application.Commands.Lookup.GetTempUser;
using CrisesControl.Api.Application.Commands.Lookup.GetTimezone;

namespace CrisesControl.Api.Application.Query
{
    public interface ILookupQuery
    {
        Task<GetImportTemplatesResponse> GetImportTemplates(GetImportTemplatesRequest request);
        Task<GetIconsResponse> GetIcons(GetIconsRequest request);
        Task<GetAllTmpUserResponse> GetAllTmpUser(GetAllTmpUserRequest request);
        Task<GetAllTmpLocResponse> GetAllTmpLoc(GetAllTmpLocRequest request);
        Task<GetAllTmpDeptResponse> GetAllTmpDept(GetAllTmpDeptRequest request);
        Task<GetTempUserResponse> GetTempUser(GetTempUserRequest request);
        Task<GetTempDeptResponse> GetTempDept(GetTempDeptRequest request);
        Task<GetTempLocResponse> GetTempLoc(GetTempLocRequest request);
        Task<AssetTypesResponse> AssetTypes();
        Task<GetTimezoneResponse> GetTimezone();
        Task<GetCountryResponse> GetCountry();
    }
}
