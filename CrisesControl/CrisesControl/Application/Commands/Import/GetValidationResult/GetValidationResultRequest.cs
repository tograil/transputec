using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.GetValidationResult
{
    public class GetValidationResultRequest: IRequest<GetValidationResultResponse>
    {
        public string SessionId { get; set; }
        public bool SendInvite { get; set; }
        public int CreatedBy { get; set; }
    }
}
