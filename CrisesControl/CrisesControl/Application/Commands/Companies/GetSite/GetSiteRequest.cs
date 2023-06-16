using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetSite
{
    public class GetSiteRequest:IRequest<GetSiteResponse>
    {
        public int SiteId { get; set; }
        public int CompanyId { get; set; }
    }
}
