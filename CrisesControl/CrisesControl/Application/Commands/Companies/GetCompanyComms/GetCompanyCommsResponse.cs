using CrisesControl.Core.Companies;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Companies.GetCompanyComms
{
    public class GetCompanyCommsResponse
    {
        public CompanyCommunication Data { get; set; }
        public HttpStatusCode ErrorCode { get; set; }
    }
}
