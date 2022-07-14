using CrisesControl.Core.Administrator;

namespace CrisesControl.Api.Application.Commands.Administrator.SendCustomerNotice
{
    public class SendCustomerNoticeResponse
    {
        public List<AdminUsersList> Data { get; set; }
        public string Message { get; set; }
    }
}
