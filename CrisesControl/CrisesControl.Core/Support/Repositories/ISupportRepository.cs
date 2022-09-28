using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Support.Repositories
{
    public interface ISupportRepository
    {
        Task<IncidentDataByActivationRefResponse> GetIncidentData(int incidentActivationId, int companyId);
        Task<SupportUserResponse> GetUser(int userId);
        Task<List<IncidentMessagesRtn>> GetIncidentReportDetails(int incidentActivationId, int companyId);

    }
}
