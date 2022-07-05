using CrisesControl.Core.Administrator;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageFeatures
{
    public class GetCompanyPackageFeaturesResponse
    {
        public List<CompanyPackageFeatureList> Feature { get; set; }
        public List<CompanyPackageFeatureList> Modules { get; set; }
        public string Message { get; set; }
    }
}
