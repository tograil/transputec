using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.SharedKernel.Utils;
using MediatR;
using Serilog;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyDRPlan
{

    public class UpdateCompanyDRPlanHandler : IRequestHandler<UpdateCompanyDRPlanRequest, UpdateCompanyDRPlanResponse>
    {
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<UpdateCompanyDRPlanHandler> _logger;
        private readonly ICompanyRepository _companyRepository;

        public UpdateCompanyDRPlanHandler(ICurrentUser currentUser, ILogger<UpdateCompanyDRPlanHandler> logger,
            ICompanyRepository companyRepository)
        {
            this._currentUser = currentUser;
            this._logger = logger;
            this._companyRepository = companyRepository;
        }

        public async Task<UpdateCompanyDRPlanResponse> Handle(UpdateCompanyDRPlanRequest request,
            CancellationToken cancellationToken)
        {

            var company = await this._companyRepository.GetCompanyByID(request.CompanyId);
            if (company != null)
            {
                company.CompanyId = request.CompanyId;
                company.PlanDrdoc = (request.DRPlan == null || request.DRPlan == string.Empty) ? "" : request.DRPlan;
                company.UpdatedOn =
                    CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(_currentUser.TimeZone);
                company.UpdatedBy = _currentUser.UserId;
                await _companyRepository.UpdateCompanyDRPlan(company);

                return new UpdateCompanyDRPlanResponse
                {
                    CompanyId = company.CompanyId,
                    Message = "Plan uploaded successfully"
                };

            }

            throw new CompanyNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
        }
    }
}
