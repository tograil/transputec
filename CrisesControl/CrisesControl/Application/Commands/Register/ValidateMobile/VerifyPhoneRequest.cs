using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.ValidateMobile
{
    public class VerifyPhoneRequest : IRequest<VerifyPhoneResponse>
    {
        public string Code { get; set; }
        public string ISD { get; set; }
        public string MobileNo { get; set; }
    }
}
