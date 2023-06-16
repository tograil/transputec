using AutoMapper;
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyGlobalReport
{
    public class GetCompanyGlobalReportHandler:IRequestHandler<GetCompanyGlobalReportRequest, GetCompanyGlobalReportResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<GetCompanyGlobalReportHandler> _logger;
        private readonly IMapper _mapper;
   

        public GetCompanyGlobalReportHandler(IAdminRepository adminRepository, ILogger<GetCompanyGlobalReportHandler> logger, IMapper mapper)
        {
            this._adminRepository = adminRepository;
            this._logger = logger;
            this._mapper = mapper;
        }

        public async Task<GetCompanyGlobalReportResponse> Handle(GetCompanyGlobalReportRequest request, CancellationToken cancellationToken)
        {
            var report = await _adminRepository.GetCompanyGlobalReport();
            var result = _mapper.Map<CompaniesStats>(report);
            var response = new GetCompanyGlobalReportResponse();
            if (result != null)
            {
                response.data = report;
            }
            else
            {
                response.data = null;
            }
            return response;
        }
    }
}
