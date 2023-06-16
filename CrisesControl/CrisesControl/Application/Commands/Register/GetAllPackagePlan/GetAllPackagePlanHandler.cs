using CrisesControl.Core.Register.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.GetAllPackagePlan
{
    public class GetAllPackagePlanHandler : IRequestHandler<GetAllPackagePlanRequest, GetAllPackagePlanResponse>
    {
        private readonly ILogger<GetAllPackagePlanHandler> _logger;
        private readonly IRegisterRepository _registerQuery;
        public GetAllPackagePlanHandler(ILogger<GetAllPackagePlanHandler> logger, IRegisterRepository registerQuery)
        {
            _logger = logger;
            _registerQuery = registerQuery;
        }
        public async Task<GetAllPackagePlanResponse> Handle(GetAllPackagePlanRequest request, CancellationToken cancellationToken)
        {
            var packagePlan = await _registerQuery.GetAllPackagePlan();
            var result = new GetAllPackagePlanResponse();
            if (packagePlan != null)
            {
                result.Data = packagePlan;
                result.Message = "Data has been Loaded";
            }
            else
            {
                result.Data = null;
                result.Message = "No record Found.";
            }
            return result;
        }
    }
}
