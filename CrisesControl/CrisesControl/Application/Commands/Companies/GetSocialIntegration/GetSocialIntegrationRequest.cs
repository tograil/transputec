using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetSocialIntegration
{
    public class GetSocialIntegrationRequest:IRequest<GetSocialIntegrationResponse>
    {
        public int CompanyID { get; set; }
        public string AccountType { get; set; }
    }
}
