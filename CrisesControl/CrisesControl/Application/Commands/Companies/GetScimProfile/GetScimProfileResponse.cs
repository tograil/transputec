using CrisesControl.Core.Companies;

namespace CrisesControl.Api.Application.Commands.Companies.GetScimProfile
{
    public class GetScimProfileResponse
    {
        public CompanyScimProfile Data { get; set; }
        public string Message { get; set; }
    }
}
