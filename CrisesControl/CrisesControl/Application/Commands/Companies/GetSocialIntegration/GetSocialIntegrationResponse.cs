using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Companies.GetSocialIntegration
{
    public class GetSocialIntegrationResponse
    {
        public List<SocialIntegraion> Data { get; set; }
        public string Message { get; set; }
    }
}
