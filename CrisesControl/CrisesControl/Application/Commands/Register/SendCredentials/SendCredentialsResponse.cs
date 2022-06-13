using CrisesControl.Core.Users;

namespace CrisesControl.Api.Application.Commands.Register.SendCredentials
{
    public class SendCredentialsResponse
    {
        public User Data { get; set; }
        public string Message { get; set; }
    }
}
