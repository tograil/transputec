using CrisesControl.Api.Application.Commands.Sop.SaveSOPHeader;

namespace CrisesControl.Api.Application.Query
{
    public interface ISopQuery
    {
        Task<SaveSOPHeaderResponse> SaveSOPHeader(SaveSOPHeaderRequest request);
    }
}
