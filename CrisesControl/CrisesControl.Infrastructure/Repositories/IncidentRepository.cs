using System.Linq;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Infrastructure.Context;

namespace CrisesControl.Infrastructure.Repositories;

public class IncidentRepository : IIncidentRepository
{
    private readonly CrisesControlContext _context;
    public IncidentRepository(CrisesControlContext context)
    {
        _context = context;
    }

    public bool CheckDuplicate(int companyId, string incidentName, int incidentId)
    {
        if (incidentId == 0)
            return !_context.Set<Incident>().Any(x => x.CompanyId == companyId 
                                                                    && x.Name == incidentName && x.Status != 3);

        return _context.Set<Incident>().Any(x => x.CompanyId == companyId
                                                 && x.Name == incidentName && x.Status != 3 && x.IncidentId == incidentId);
    }
}