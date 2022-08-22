using CrisesControl.Api.Application.Commands.System.ExportTrackingData;
using CrisesControl.Api.Application.Commands.System.ViewErrorLog;
using CrisesControl.Api.Application.Commands.System.ViewModelLog;

namespace CrisesControl.Api.Application.Query
{
    public interface ISystemQuery
    {
        Task<ExportTrackingDataResponse> ExportTrackingData(ExportTrackingDataRequest request);
        Task<ViewModelLogResponse> ViewModelLog(ViewModelLogRequest request);
        Task<ViewErrorLogResponse> ViewErrorLog(ViewErrorLogRequest request);
    }
}
