using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.ActivateCompany
{
    public class ActivateCompanyRequest : IRequest<ActivateCompanyResponse>
    {
        public int UserId { get; set; }
        public string IPAddress { get; set; }
        public string ActivationKey { get; set; }
        public int SalesSource { get; set; }
    }
}
