using CrisesControl.Core.Register;

namespace CrisesControl.Api.Application.Commands.Register.GetAllPackagePlan
{
    public class GetAllPackagePlanResponse
    {
        public List<PackageModel> Data { get; set; }
        public string Message { get; set; }
    }
}
