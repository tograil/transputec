using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyDRPlan
{
    public class UpdateCompanyDRPlanHandler : IRequestHandler<UpdateCompanyDRPlanRequest, UpdateCompanyDRPlanResponse>
    {
        public Task<UpdateCompanyDRPlanResponse> Handle(UpdateCompanyDRPlanRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
