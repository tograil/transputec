namespace CrisesControl.Api.Application.Commands.Users.ValidateEmail
{
    public class ValidateEmailResponse
    {
        public string SSOType { get; set; }
        public string SSOEnabled {get;set;}
        public string SSOIssuer { get; set; }
        public string SSOSecret { get; set; }
    }
}
