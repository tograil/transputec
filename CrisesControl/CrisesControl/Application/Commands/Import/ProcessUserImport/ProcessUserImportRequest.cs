using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.ProcessUserImport
{
    public class ProcessUserImportRequest:IRequest<ProcessUserImportResponse>
    {
        public string SessionId { get; set; }
        public bool SendInvite { get; set; }
        public int CreatedBy { get; set; }
    }
}
