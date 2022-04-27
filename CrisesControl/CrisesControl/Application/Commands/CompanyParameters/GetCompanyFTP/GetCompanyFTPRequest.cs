using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyFTP
{
    public class GetCompanyFTPRequest: IRequest<GetCompanyFTPResponse>
    {
      
        public int CompanyID { get; set; }
    }
}
