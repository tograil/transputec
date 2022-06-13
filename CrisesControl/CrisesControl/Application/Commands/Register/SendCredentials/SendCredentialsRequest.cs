using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.SendCredentials
{
    public class SendCredentialsRequest:IRequest<SendCredentialsResponse>
    {
        public string UniqueId { get; set; }
    }
}
