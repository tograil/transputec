using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.RemoveSection
{
    public class RemoveSectionRequest:IRequest<RemoveSectionResponse>
    {
        public int ContentSectionID { get; set; }
       
    }
}
