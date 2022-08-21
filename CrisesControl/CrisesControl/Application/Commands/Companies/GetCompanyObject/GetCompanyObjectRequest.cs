using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetCompanyObject
{
    public class GetCompanyObjectRequest:IRequest<GetCompanyObjectResponse>
    {
        public string ObjectName { get; set; }
    }
}
