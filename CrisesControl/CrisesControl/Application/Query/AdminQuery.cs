using AutoMapper;
using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncident;
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Administrator.Repositories;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query
{
    public class AdminQuery: IAdminQuery
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AdminQuery> _logger;
        private readonly IAdminRepository _adminRepository;
        public AdminQuery(IMapper mapper, ILogger<AdminQuery> logger, IAdminRepository administratorRepository)
        {
            this._logger=logger;
            this._mapper=mapper;
            this._adminRepository=administratorRepository;
        }

        public async Task<GetAllLibIncidentResponse> GetAllLibIncident(GetAllLibIncidentRequest request)
        {
            var libIncidents = await _adminRepository.GetAllLibIncident();
            var response = _mapper.Map<List<LibIncident>>(libIncidents);
            var result = new GetAllLibIncidentResponse();
            result.data = response;
            result.Message = "Data loaded Successfully";
            return result;
        }
    }
}
