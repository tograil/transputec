using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.SaveSOPHeader
{
    public class SaveSOPHeaderRequest:IRequest<SaveSOPHeaderResponse>
    {
        public SaveSOPHeaderRequest()
        {
            SOPHeaderID = 0;
        }
        public int SOPHeaderID { get; set; }
        public int IncidentID { get; set; }
        public int ContentID { get; set; }
        public int ContentSectionID { get; set; }
        public string ContentText { get; set; }
        public string SOPVersion { get; set; }
        public int SOPOwner { get; set; }
        public DateTimeOffset ReviewDate { get; set; }
        public string ReviewFrequency { get; set; }
        public string SOPFileName { get; set; }
        public int AssetID { get; set; }
        public int Status { get; set; }
    }
}
