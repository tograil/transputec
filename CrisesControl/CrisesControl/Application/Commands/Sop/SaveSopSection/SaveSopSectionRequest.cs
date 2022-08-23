using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.SaveSopSection
{
    public class SaveSopSectionRequest:IRequest<SaveSopSectionResponse>
    {
        public int SOPHeaderID { get; set; }
        public int ContentID { get; set; }
        public int ContentSectionID { get; set; }
        public string SectionType { get; set; }
        public string SectionDescription { get; set; }
        public int SectionOrder { get; set; }
        public List<int> SOPContentTags { get; set; }
        public List<int> SOPGroups { get; set; }
        public string SectionName { get; set; }
        public int SectionStatus { get; set; }
    }
}
