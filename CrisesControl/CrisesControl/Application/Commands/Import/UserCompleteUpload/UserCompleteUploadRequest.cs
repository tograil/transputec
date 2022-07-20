using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.UserCompleteUpload
{
    public class UserCompleteUploadRequest:IRequest<UserCompleteUploadResponse>
    {
        public bool ImportAsActive { get; set; }
        public UserCompleteUploadData[] UserCompleteUploadData { get; set; }
        public int UserImportTotalId { get; set; }
        public string SessionId { get; set; }
    }

    public class UserCompleteUploadData
    {
        public int UserImportTotalId { get; set; }
        public string ImportAction { get; set; }
    }
}
