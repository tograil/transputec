using CrisesControl.Core.Companies;

namespace CrisesControl.Api.Application.Commands.Lookup.GetTempUser
{
    public class GetTempUserResponse
    {
        public Registration Data { get; set; }
        public string Message { get; set; }
    }
}
