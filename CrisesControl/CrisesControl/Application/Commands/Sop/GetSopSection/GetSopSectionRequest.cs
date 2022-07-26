using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.GetSopSection
{
    public class GetSopSectionRequest:IRequest<GetSopSectionResponse>
    {
        public int SOPHeaderID { get; set; }
        public int ContentSectionID { get; set; }
    }
}
