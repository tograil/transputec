using AutoMapper;
using CrisesControl.Api.Application.Commands.Academy.GetToursSteps;
using CrisesControl.Api.Application.Commands.Academy.GetUserVideos;
using CrisesControl.Api.Application.Commands.Academy.GetVideos;
using CrisesControl.Api.Application.Commands.Academy.SaveTourLog;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.Core.Academy;
using CrisesControl.Core.Academy.Repositories;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query
{
    public class AcademyQuery : IAcademyQuery
    {
        private readonly IAcademyRepository _academyRepository;
        private readonly ILogger<AcademyQuery> _logger;
        private readonly ICurrentUser _currentUser;
        //private readonly IMapper _mapper;
        private readonly IPaging _paging;
        public AcademyQuery(IAcademyRepository academyRepository, ILogger<AcademyQuery> logger, ICurrentUser currentUser, /*IMapper mapper,*/ IPaging paging)
        {
            this._academyRepository = academyRepository;
            this._logger = logger;
            this._currentUser = currentUser;
           // this._mapper = mapper;
            this._paging = paging;
        }
        public async Task<GetToursStepsResponse> GetToursSteps(GetToursStepsRequest request)
        {
            try
            {

                var tourSteps = await _academyRepository.GetToursSteps();
               // var result = _mapper.Map<List<TourStep>>(tourSteps);
                var response = new GetToursStepsResponse();
                if (tourSteps != null)
                {
                    response.Data = tourSteps;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = null;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetUserVideosResponse> GetUserVideos(GetUserVideosRequest request)
        {
            try
            {

                var videos = await _academyRepository.GetVideos(_currentUser.UserId);
                //var result = _mapper.Map<List<AcademyVideos>>(videos);
                var response = new GetUserVideosResponse();
                if (videos != null)
                {
                    response.Data = videos;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = null;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetVideosResponse> GetVideos(GetVideosRequest request)
        {
            try
            {

                var videos = await _academyRepository.GetVideos(0);
               // var result = _mapper.Map<List<AcademyVideos>>(videos);
                var response = new GetVideosResponse();
                if (videos != null)
                {
                    response.Data = videos;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = null;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SaveTourLogResponse> SaveTourLog(SaveTourLogRequest request)
        {
            try
            {

                var tourId= await _academyRepository.SaveTourLog(_currentUser.UserId,request.TourName,request.StepKey,_currentUser.TimeZone);
               // var result = _mapper.Map<int>(tourId);
                var response = new SaveTourLogResponse();
                if (tourId > 0)
                {
                    response.TourLogId = tourId;
                    response.Message = "Tour Log Added";
                }
                else
                {
                    response.TourLogId = tourId;
                    response.Message = "No Tour Log Added.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
