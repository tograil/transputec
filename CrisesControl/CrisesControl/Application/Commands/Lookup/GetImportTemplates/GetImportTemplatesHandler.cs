using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetImportTemplates
{
    public class GetImportTemplatesHandler : IRequestHandler<GetImportTemplatesRequest, GetImportTemplatesResponse>
    {
        private readonly ILookupQuery _lookupQuery;
        private readonly ILogger<GetImportTemplatesHandler> _logger;
        public GetImportTemplatesHandler(ILogger<GetImportTemplatesHandler> logger, ILookupQuery lookupQuery)
        {
            this._logger = logger;
            this._lookupQuery = lookupQuery;
        }
        public async Task<GetImportTemplatesResponse> Handle(GetImportTemplatesRequest request, CancellationToken cancellationToken)
        {
            var result = await _lookupQuery.GetImportTemplates(request);
            return result;
        }
    }
}
