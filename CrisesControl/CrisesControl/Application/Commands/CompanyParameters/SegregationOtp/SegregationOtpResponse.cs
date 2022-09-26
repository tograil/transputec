using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Import;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.SegregationOtp
{
    public class SegregationOtpResponse : CommonDTO
    {
        public OTPResponse Data { get; set; }
    }
}
