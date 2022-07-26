using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Academy.GetToursSteps
{
    public class GetToursStepsHandler : IRequestHandler<GetToursStepsRequest, GetToursStepsResponse>
    {
        private readonly IAcademyQuery _academyQuery;
        private readonly ILogger<GetToursStepsHandler> _logger;
       
        public GetToursStepsHandler(IAcademyQuery academyQuery, ILogger<GetToursStepsHandler> logger)
        {
            this._academyQuery = academyQuery;
            this._logger = logger;
           
        }

        public async Task<GetToursStepsResponse> Handle(GetToursStepsRequest request, CancellationToken cancellationToken)
        {
            var result = await _academyQuery.GetToursSteps(request);
            return result;
        }
    }
}
