using CrisesControl.Api.Application.Commands.SopLibrary.DeleteSOPLib;
using CrisesControl.Api.Application.Commands.SopLibrary.GetSopSection;
using CrisesControl.Api.Application.Commands.SopLibrary.GetSopSections;
using CrisesControl.Api.Application.Commands.SopLibrary.SaveLibSection;
using CrisesControl.Api.Application.Commands.SopLibrary.UseLibraryText;

namespace CrisesControl.Api.Application.Query
{
    public interface ISopLibraryQuery
    {
        Task<GetSopSectionsResponse> GetSopSections(GetSopSectionsRequest request);
        Task<SaveLibSectionResponse> SaveLibSection(SaveLibSectionRequest request);
        Task<GetSopSectionResponse> GetSopSection(GetSopSectionRequest request);
        Task<UseLibraryTextResponse> UseLibraryText(UseLibraryTextRequest request);
        Task<DeleteSOPLibResponse> DeleteSOPLib(DeleteSOPLibRequest request);
    }
}
