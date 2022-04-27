using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Communication.Repositories
{
    public interface ICommunicationRepository
    {
        Task<IEnumerable<ConferenceDetails>> GetUserActiveConferenceList(int UserID, int CompanyID);
    }
}
