using MediatR;

namespace CrisesControl.Api.Application.Commands.SopLibrary.SaveLibSection
{
    public class SaveLibSectionRequest:IRequest<SaveLibSectionResponse>
    {
        public int SOPHeaderID { get; set; }
        public int IncidentID { get; set; }
        public int ContentID { get; set; }
        public int ContentSectionID { get; set; }
        public string SectionDescription { get; set; }
        public List<int> SOPContentTags { get; set; }
        public string SectionName { get; set; }
        public int SectionStatus { get; set; }
        public string SOPVersion { get; set; }
        public DateTimeOffset ReviewDate { get; set; }
    }
}
