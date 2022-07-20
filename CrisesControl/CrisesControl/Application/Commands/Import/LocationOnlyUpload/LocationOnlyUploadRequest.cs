using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.LocationOnlyUpload
{
    public class LocationOnlyUploadRequest: IRequest<LocationOnlyUploadResponse>
    {
        public string SessionId { get; set; }
        public int UserImportTotalId { get; set; }
        public LocationUploadData[] LocationUploadData { get; set; }
    }

    public class LocationUploadData
    {
        public int UserImportTotalId { get; set; }
        public string ImportAction { get; set; }
    }
}
