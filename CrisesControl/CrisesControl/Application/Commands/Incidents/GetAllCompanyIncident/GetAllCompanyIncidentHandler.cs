using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;
namespace CrisesControl.Api.Application.Commands.Incidents.GetAllCompanyIncident
{
    public class GetAllCompanyIncidentHandler:IRequestHandler<GetAllCompanyIncidentRequest, GetAllCompanyIncidentResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetAllCompanyIncidentHandler> _logger;
        private readonly GetAllCompanyIncidentValidator _getAllCompanyIncidentValidator;
        public GetAllCompanyIncidentHandler(IIncidentQuery incidentQuery, ILogger<GetAllCompanyIncidentHandler> logger, GetAllCompanyIncidentValidator getAllCompanyIncidentValidator)
        {
            _incidentQuery = incidentQuery;
            _logger = logger;
            _getAllCompanyIncidentValidator = getAllCompanyIncidentValidator;
        }

        public async Task<GetAllCompanyIncidentResponse> Handle(GetAllCompanyIncidentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAllCompanyIncidentRequest));
            await _getAllCompanyIncidentValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result=await  _incidentQuery.GetAllCompanyIncident(request);
            return result;
        }
    }
}
