using CrisesControl.Core.Users;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.StartConference
{
    public class StartConferenceRequest : IRequest<StartConferenceResponse>
    {
        public List<User> UserList { get; set; }
        public int ObjectID { get; set; }
        //public int CurrentUserID { get; set; }
        //public int CompanyID { get; set; }
        //public string TimeZoneId { get; set; }
    }
}
