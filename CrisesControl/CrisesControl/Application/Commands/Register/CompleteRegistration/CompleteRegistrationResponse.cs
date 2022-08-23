namespace CrisesControl.Api.Application.Commands.Register.CompleteRegistration
{
    public class CompleteRegistrationResponse
    {
        public int companyid { get; set; }
        public int userid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public int ErrorId { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string CustomerId { get; set; }
    }
}
