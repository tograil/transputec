using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.CreateSampleIncident
{
    public class CreateSampleIncidentHandler : IRequestHandler<CreateSampleIncidentRequest, CreateSampleIncidentResponse>
    {
        private readonly ILogger<CreateSampleIncidentHandler> _logger;
        private readonly IRegisterQuery _registerQuery;
        private readonly CreateSampleIncidentValidator _createSampleIncidentValidator;
        public CreateSampleIncidentHandler(ILogger<CreateSampleIncidentHandler> logger, IRegisterQuery registerQuery, CreateSampleIncidentValidator createSampleIncidentValidator)
        {
            this._logger = logger;
            this._registerQuery = registerQuery;
            this._createSampleIncidentValidator = createSampleIncidentValidator;
        }
        public async Task<CreateSampleIncidentResponse> Handle(CreateSampleIncidentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateSampleIncidentRequest));

            await _createSampleIncidentValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _registerQuery.CreateSampleIncident(request);
            return result;
        }
    }
}
