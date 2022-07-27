using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.DeleteSOP
{
    public class DeleteSOPRequest:IRequest<DeleteSOPResponse>
    {
        public int SOPHeaderID { get; set; }
    }
}
