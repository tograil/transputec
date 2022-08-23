using CrisesControl.Api.Application.Commands.Sop.AttachSOPToIncident;
using CrisesControl.Api.Application.Commands.Sop.DeleteSOP;
using CrisesControl.Api.Application.Commands.Sop.GetCompanySOP;
using CrisesControl.Api.Application.Commands.Sop.GetSopSection;
using CrisesControl.Api.Application.Commands.Sop.GetSOPSectionLibrary;
using CrisesControl.Api.Application.Commands.Sop.GetSopSections;
using CrisesControl.Api.Application.Commands.Sop.GetTagList;
using CrisesControl.Api.Application.Commands.Sop.LibraryTextModel;
using CrisesControl.Api.Application.Commands.Sop.RemoveSection;
using CrisesControl.Api.Application.Commands.Sop.ReorderSection;
using CrisesControl.Api.Application.Commands.Sop.SaveSOPHeader;
using CrisesControl.Api.Application.Commands.Sop.SaveSopSection;
using CrisesControl.Api.Application.Commands.Sop.UpdateSOPAsset;

namespace CrisesControl.Api.Application.Query
{
    public interface ISopQuery
    {
        Task<SaveSOPHeaderResponse> SaveSOPHeader(SaveSOPHeaderRequest request);
        Task<GetTagListResponse> GetTagList(GetTagListRequest request);
        Task<GetCompanySOPResponse> GetCompanySOP(GetCompanySOPRequest request);
        Task<GetSopSectionsResponse> GetSopSections(GetSopSectionsRequest request);
        Task<SaveSopSectionResponse> SaveSopSection(SaveSopSectionRequest request);
        Task<RemoveSectionResponse> RemoveSection(RemoveSectionRequest request);
        Task<GetSopSectionResponse> GetSopSection(GetSopSectionRequest request);
        Task<ReorderSectionResponse> ReorderSection(ReorderSectionRequest request);
        Task<GetSOPSectionLibraryResponse> GetSOPSectionLibrary(GetSOPSectionLibraryRequest request);
        Task<AttachSOPToIncidentResponse> AttachSOPToIncident(AttachSOPToIncidentRequest request);
        Task<LibraryTextModelResponse> LibraryTextModel(LibraryTextModelRequest request);
        Task<DeleteSOPResponse> DeleteSOP(DeleteSOPRequest request);
        Task<UpdateSOPAssetResponse> UpdateSOPAsset(UpdateSOPAssetRequest request);
    }
}
