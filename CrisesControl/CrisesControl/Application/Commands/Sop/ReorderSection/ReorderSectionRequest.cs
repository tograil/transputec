using CrisesControl.Core.Sop;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.ReorderSection
{
    public class ReorderSectionRequest:IRequest<ReorderSectionResponse>
    {
        public List<Section> SectionOrder { get; set; }
    }
}
