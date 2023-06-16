using System.Net;

namespace CrisesControl.Api.Application.Commands.Companies.SaveSite
{
    public class SaveSiteResponse
    {
        public int SiteId { get; set; }
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode ErrorCode { get; set; }
    }
}
