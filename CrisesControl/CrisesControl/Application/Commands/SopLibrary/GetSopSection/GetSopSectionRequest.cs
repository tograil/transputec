using MediatR;

namespace CrisesControl.Api.Application.Commands.SopLibrary.GetSopSection
{
    public class GetSopSectionRequest:IRequest<GetSopSectionResponse>
    {
        public int SOPHeaderID { get; set; }
    }
}
