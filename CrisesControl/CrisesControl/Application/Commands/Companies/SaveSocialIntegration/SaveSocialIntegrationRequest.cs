using CrisesControl.Core.Models;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.SaveSocialIntegration
{
    public class SaveSocialIntegrationRequest:IRequest<SaveSocialIntegrationResponse>
    {
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string AuthToken { get; set; }
        public string AuthSecret { get; set; }
        public string AdnlKeyOne { get; set; }
        public string AdnlKeyTwo { get; set; }
    }
}
