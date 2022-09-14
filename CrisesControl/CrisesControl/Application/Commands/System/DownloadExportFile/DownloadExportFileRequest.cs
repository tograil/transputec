using MediatR;

namespace CrisesControl.Api.Application.Commands.System.DownloadExportFile
{
    public class DownloadExportFileRequest:IRequest<DownloadExportFileResponse>
    {
        public int CompanyId { get; set; }
        public string FileName { get; set; }
    }
}
