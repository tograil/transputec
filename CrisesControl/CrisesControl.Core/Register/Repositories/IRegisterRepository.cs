using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Register.Repositories
{
    public interface IRegisterRepository
    {
        Task<bool> CheckCustomer(string CustomerId);
        Task<string>  VerifyPhone(string Code, string ISD, string MobileNo, string Message = "");
        Task<CommsStatus> SendText(string ISD, string ToNumber, string Message, string CallbackUrl = "");
        Task<CommsStatus> VerificationCall(string mobileNo, string message, bool sendInDirect, string twilioRoutingApi);
    }
}
