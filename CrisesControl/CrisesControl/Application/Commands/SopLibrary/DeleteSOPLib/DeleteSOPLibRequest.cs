using MediatR;

namespace CrisesControl.Api.Application.Commands.SopLibrary.DeleteSOPLib
{
    public class DeleteSOPLibRequest:IRequest<DeleteSOPLibResponse>
    {
        public DeleteSOPLibRequest()
        {
            SOPHeaderID = 0;
        }
        public int SOPHeaderID { get; set; }
    }
}
