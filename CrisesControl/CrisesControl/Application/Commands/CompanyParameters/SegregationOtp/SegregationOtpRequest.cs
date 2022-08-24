using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.SegregationOtp
{
    public class SegregationOtpRequest:IRequest<SegregationOtpResponse>
    {
        public string Method { get; set; }
    }
}
