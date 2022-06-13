namespace CrisesControl.Api.Application.Commands.Register.ValidateUserEmail
{
    public class ValidateUserEmailResponse
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email  { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
