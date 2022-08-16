using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrisesControl.Core.Communication.Repositories {
    public interface ICommunicationRepository {
        public Task<List<UserConferenceItem>> GetUserActiveConferences();
        Task<string> HandelCallResponse(string CallSid, string CallStatus, string From, string To, int Duration = 0, string Operator = "TWILIO");
    }
}
