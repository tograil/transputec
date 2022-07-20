using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.GetImportResult
{
    public class GetImportResultRequest : IRequest<GetImportResultResponse>
    {
        public string SessionId { get; set; }
        public bool SendInvite { get; set; }
        public int CreatedBy { get; set; }
    }
}
