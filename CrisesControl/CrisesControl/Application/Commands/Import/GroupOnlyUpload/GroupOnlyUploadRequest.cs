using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.GroupOnlyUpload
{
    public class GroupOnlyUploadRequest:IRequest<GroupOnlyUploadResponse>
    {
        public int UserImportTotalId { get; set; }
    }
}
