using CrisesControl.Core.Users;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrisesControl.Api.Application.Commands.Communication.StartConference
{
    public class StartConferenceRequest:IRequest<StartConferenceResponse>
    {
        

        /// <summary>
        /// UserId of the logged in user, sent in return, need to be sent with TOKEN for subsequent logins
        /// </summary>
        [Required(ErrorMessage = "User ID can not be blank")]
        public int UserId { get; set; }


        /// <summary>
        /// UserId of the logged in user, sent in return, need to be sent with TOKEN for subsequent logins
        /// </summary>
     
        public int CompanyId { get; set; }

        /// <summary>
        /// Array of users ids as a sub list
        /// </summary>
        [Required(ErrorMessage = "All user ids of selected users for conf.")]
        [NotMapped]
        public List<User> ConfUsers { get; set; }

        /// <summary>
        /// Active Incident Id
        /// </summary>
        [Required(ErrorMessage = "Active Incident id is required")]
        public int ActiveIncidentId { get; set; }

        /// <summary>
        /// Incident Message Id (optional), please pass the message id when starting the conference from the message ack list, else pass 0.
        /// </summary>
        public int IncidentMessageId { get; set; }
    }
}
