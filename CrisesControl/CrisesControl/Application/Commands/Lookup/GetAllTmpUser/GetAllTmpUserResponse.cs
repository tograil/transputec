using CrisesControl.Core.Companies;

namespace CrisesControl.Api.Application.Commands.Lookup.GetAllTmpUser
{
    public class GetAllTmpUserResponse
    {
        public List<Registration> Data { get; set; }
        public string Message { get; set; }
    }
}
