using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.NoAppUser
{
    public class NoAppUserRequest:IRequest<NoAppUserResponse>
    {
        public int CompanyId { get; set; }
        public int MessageID { get; set; }
        public string CompanyKey { get; set; }
        public string search { get; set; }
        public string orderDir { get; set; }
        public int Draw { get; set; }
        
    }
}
