using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.TestWithMe
{
    public class TestWithMeHandler : IRequestHandler<TestWithMeRequest, TestWithMeResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<TestWithMeHandler> _logger;
        public TestWithMeHandler(IIncidentQuery incidentQuery, ILogger<TestWithMeHandler> logger)
        {
            this._incidentQuery = incidentQuery;
            this._logger = logger;
        }
        public async Task<TestWithMeResponse> Handle(TestWithMeRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.TestWithMe(request);
            return result;
        }
    }
}
