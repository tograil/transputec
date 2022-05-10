using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyFTP
{
    public class GetCompanyFTPResponse
    {
        public List<CompanyFtp> Data { get; set; }
        public string ErrorCode { get; set; }
    }
}
