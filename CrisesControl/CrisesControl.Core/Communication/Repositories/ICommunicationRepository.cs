using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrisesControl.Core.Communication.Repositories {
    public interface ICommunicationRepository {
        public Task<List<UserConferenceItem>> GetUserActiveConferences();
    }
}
