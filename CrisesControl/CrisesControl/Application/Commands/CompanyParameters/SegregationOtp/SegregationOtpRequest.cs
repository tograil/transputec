using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.SegregationOtp
{
    public class SegregationOtpRequest : IRequest<SegregationOtpResponse>
    {
        public int CompanyId { get; set; }
        public int CurrentUserId { get; set; }
        public string Method { get; set; }

    }
}
