using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrisesControl.Core.CompanyAggregate.Services;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CrisesControl.Infrastructure.Services;

public class CompanyService : ICompanyService
{
    private readonly CrisesControlContext _context;

    public CompanyService(CrisesControlContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Company>> GetAllCompanies()
    {
        try
        {
            return await _context.Set<Company>().AsNoTracking().ToArrayAsync();
        }
        catch (InvalidCastException exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }
}