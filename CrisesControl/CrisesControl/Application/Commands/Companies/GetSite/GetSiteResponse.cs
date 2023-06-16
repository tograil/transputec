using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Companies.GetSite
{
    public class GetSiteResponse
    {
        public Site site { get; set; }
        public List<Site> sites { get; set; }
        public string Message { get; set; }
    }
}
