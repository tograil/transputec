using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.SaveSopSection
{
    public class SaveSopSectionHandler : IRequestHandler<SaveSopSectionRequest, SaveSopSectionResponse>
    {
        private readonly ISopQuery _sopQuery;
        public SaveSopSectionHandler(ISopQuery sopQuery)
        {
            _sopQuery = sopQuery;
        }
        public async Task<SaveSopSectionResponse> Handle(SaveSopSectionRequest request, CancellationToken cancellationToken)
        {
            var result = await _sopQuery.SaveSopSection(request);
            return result;
        }
    }
}
