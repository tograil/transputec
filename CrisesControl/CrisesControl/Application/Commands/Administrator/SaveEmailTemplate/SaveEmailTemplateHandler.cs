using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.SaveEmailTemplate
{
    public class SaveEmailTemplateHandler : IRequestHandler<SaveEmailTemplateRequest, SaveEmailTemplateResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<SaveEmailTemplateHandler> _logger;
        public SaveEmailTemplateHandler(IAdminQuery adminQuery, ILogger<SaveEmailTemplateHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
        }
        public async Task<SaveEmailTemplateResponse> Handle(SaveEmailTemplateRequest request, CancellationToken cancellationToken)
        {
            var result = await _adminQuery.SaveEmailTemplate(request);
            return result;
        }
    }
}
