using CrisesControl.Api.Application.Commands.Common;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Incidents.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.AddCompanyIncident;

public class AddCompanyIncidentHandler : IRequestHandler<AddCompanyIncidentRequest, ResultResponse>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly ICurrentUser _currentUser;

    public AddCompanyIncidentHandler(IIncidentRepository incidentRepository, ICurrentUser currentUser)
    {
        _incidentRepository = incidentRepository;
        _currentUser = currentUser;
    }

    public Task<ResultResponse> Handle(AddCompanyIncidentRequest request, CancellationToken cancellationToken)
    {
        var user = _currentUser.CompanyId;
        throw new NotImplementedException();
    }
}