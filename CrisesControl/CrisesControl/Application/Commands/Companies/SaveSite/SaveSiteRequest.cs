using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.SaveSite
{
    public class SaveSiteRequest:IRequest<SaveSiteResponse>
    {
        public int SiteID { get; set; }
        public string SiteName { get; set; }
        public string SiteCode { get; set; }
        public int Status { get; set; }
        public int CompanyID { get; set; }
    }
}
