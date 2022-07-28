using MediatR;

namespace CrisesControl.Api.Application.Commands.SopLibrary.UseLibraryText
{
    public class UseLibraryTextRequest:IRequest<UseLibraryTextResponse>
    {
        public int SOPHeaderID { get; set; }
    }
}
