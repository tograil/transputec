using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.QueueImportJob
{
    public class QueueImportJobRequest:IRequest<QueueImportJobResponse>
    {
        public string MappingFileName { get; set; }
        public string DataFileName { get; set; }
        public string SessionId { get; set; }
        public bool SendInvite { get; set; }
        public string JobType { get; set; }
    }
}
