using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.VerifyTempRegistration
{
    public class VerifyTempRegistrationRequest:IRequest<VerifyTempRegistrationResponse>
    {
        public VerifyTempRegistrationRequest()
        {
            CountryCode = "GBR";
            RegAction = "SAVE";
        }
     
       
        public string CountryCode { get; set; }
        public string UniqueRef { get; set; }
       
        public string RegAction { get; set; }

    }
}
