using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.DownloadImportResult
{
    public class DownloadImportResultRequest : IRequest<DownloadImportResultResponse>
    {
        public int CompanyId { get; set; }
        public int SessionId { get; set; }
    }
}
