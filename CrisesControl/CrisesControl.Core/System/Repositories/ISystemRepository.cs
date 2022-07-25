using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.System.Repositories
{
    public  interface ISystemRepository
    {
        Task<string> ExportTrackingData(int TrackMeID, int UserDeviceID, DateTimeOffset StartDate, DateTimeOffset EndDate, int OutUserCompanyId);
        Task<List<ModelLogReturn>> GetModelLog(DateTimeOffset StartDate, DateTimeOffset EndDate, int RecordStart, int RecordLength, string SearchString, string OrderBy, string OrderDir);
    }
}
