using AutoMapper;
using CrisesControl.Api.Application.Commands.ExTriggers.GetAllExTrigger;
using CrisesControl.Api.Application.Commands.ExTriggers.GetExTrigger;
using CrisesControl.Api.Application.Commands.ExTriggers.GetImpTrigger;
using CrisesControl.Core.ExTriggers;
using CrisesControl.Core.ExTriggers.Repositories;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query
{
    public class ExTriggerQuery : IExTriggerQuery
    {
        private readonly IExTriggerRepository _exTriggerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ExTriggerQuery> _logger;
        public ExTriggerQuery(IExTriggerRepository exTriggerRepository, IMapper mapper, ILogger<ExTriggerQuery> logger)
        {
            this._mapper = mapper;
            this._logger = logger;
            this._exTriggerRepository= exTriggerRepository;
        }
        /// <inheritdoc/>
        public async Task<GetAllExTriggerResponse> GetAllExTrigger(GetAllExTriggerRequest request)
        {
            var triggers = await _exTriggerRepository.GetAllExTrigger(request.CompanyID, request.UserID);
            var response = _mapper.Map<List<ExTriggerList>>(triggers);
            var result = new GetAllExTriggerResponse();            
            if (triggers != null)
            {
                result.Data = response;
                result.ErrorCode = 200;
                result.Message = "Data Loaded Successfully ";
                return result;
                
            }
            result.ErrorCode = 500;
            result.Message = "No data Found";
            return result;


        }

        public async Task<GetExTriggerResponse> GetExTrigger(GetExTriggerRequest request)
        {
           var exTrigger = await _exTriggerRepository.GetExTrigger( request.ExTriggerID, request.CompanyID);

            var response = _mapper.Map<List<ExTriggerList>>(exTrigger);
            var result = new GetExTriggerResponse();
            if (exTrigger != null)
            {
                result.Data = response;
                result.ErrorCode = 200;
                result.Message = "Data Loaded Successfully ";
                return result;
            }
            result.Data = response;
            result.ErrorCode = 500;
            result.Message = "No data Found";
            return result;
        }

        public async Task<GetImpTriggerResponse> GetImpTrigger(GetImpTriggerRequest request)
        {
            var importsT = await _exTriggerRepository.GetImpTrigger(request.CompanyID, request.UserID);
         
            var response = _mapper.Map<List<ExTriggerList>>(importsT);
            var result = new GetImpTriggerResponse();
            if (importsT != null) { 
                result.Data = response;
                result.ErrorCode = 200;
                result.Message = "Data Loaded Successfully ";
                return result;
            }
            result.Data = response;
            result.ErrorCode = 500;
            result.Message = "No data Found";
            return result;
        }
    }
}
