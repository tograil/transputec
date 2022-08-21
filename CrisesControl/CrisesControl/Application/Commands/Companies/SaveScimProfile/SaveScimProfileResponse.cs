using CrisesControl.Core.Companies;

namespace CrisesControl.Api.Application.Commands.Companies.SaveScimProfile
{
    public class SaveScimProfileResponse
    {
        public CompanyScimProfile Data { get; set; }
        public string Message { get; set; }
    }
}
