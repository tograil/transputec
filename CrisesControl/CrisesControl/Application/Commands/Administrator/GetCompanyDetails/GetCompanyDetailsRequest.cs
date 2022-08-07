using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyDetails
{
    public class GetCompanyDetailsRequest:IRequest<GetCompanyDetailsResponse>
    {
        public int CompanyID { get; set; }
    }
}
