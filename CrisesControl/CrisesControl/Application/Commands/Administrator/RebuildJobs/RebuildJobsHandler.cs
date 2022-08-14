using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.RebuildJobs
{
    public class RebuildJobsHandler : IRequestHandler<RebuildJobsRequest, RebuildJobsResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<RebuildJobsHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public RebuildJobsHandler(IAdminRepository adminRepository, ICurrentUser currentUser, ILogger<RebuildJobsHandler> logger, IMapper mapper)
        {
            this._adminRepository = adminRepository;
            this._logger = logger;
            this._mapper = mapper;
        }
        public async Task<RebuildJobsResponse> Handle(RebuildJobsRequest request, CancellationToken cancellationToken)
        {
            var Jobs = await _adminRepository.RebuildJobs(request.Company, request.JobType);
            var result = _mapper.Map<bool>(Jobs);
            var response = new RebuildJobsResponse();
            if (result)
            {
                response.result = true;
                response.Message = "Job Rebuilded";
            }
            else
            {
                response.result = true;
                response.Message = "No record found.";
            }
            return response;
        }
    }
}
