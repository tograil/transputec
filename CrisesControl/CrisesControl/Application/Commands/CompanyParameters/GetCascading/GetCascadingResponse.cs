using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetCascading
{
    public class GetCascadingResponse
    {
        public List<CascadingPlanReturn> Data { get; set; }
        public string ErrorCode { get; set; }
    }
}
