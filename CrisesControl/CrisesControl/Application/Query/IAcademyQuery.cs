using CrisesControl.Api.Application.Commands.Academy.GetToursSteps;
using CrisesControl.Api.Application.Commands.Academy.GetUserVideos;
using CrisesControl.Api.Application.Commands.Academy.GetVideos;
using CrisesControl.Api.Application.Commands.Academy.SaveTourLog;

namespace CrisesControl.Api.Application.Query
{
    public interface IAcademyQuery
    {
        Task<GetVideosResponse> GetVideos(GetVideosRequest request);
        Task<GetUserVideosResponse> GetUserVideos(GetUserVideosRequest request);
        Task<GetToursStepsResponse> GetToursSteps(GetToursStepsRequest request);
        Task<SaveTourLogResponse> SaveTourLog(SaveTourLogRequest request);
        
    }
}
