using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.DepartmentOnlyUpload
{
    public class DepartmentOnlyUploadRequest : IRequest<DepartmentOnlyUploadResponse>
    {
        public int UserImportTotalId { get; set; }
    }
}
