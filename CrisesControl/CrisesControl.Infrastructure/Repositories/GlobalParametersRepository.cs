using System;
using System.Collections.Generic;
using System.Linq;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CrisesControl.Infrastructure.Repositories;

public class GlobalParametersRepository : IGlobalParametersRepository
{
    private readonly CrisesControlContext _context;

    private readonly Lazy<ICollection<GlobalParams>> _globalParams;

    public GlobalParametersRepository(CrisesControlContext context)
    {
        _context = context;
        _globalParams = new Lazy<ICollection<GlobalParams>>(()
            => _context.Set<GlobalParams>().FromSqlRaw("exec Pro_Global_GetSystemParameter {0}", string.Empty).ToArray());
    }

    public IEnumerable<GlobalParams> GlobalParams => _globalParams.Value;

    public string LookupWithKey(string key, string defaults = "")
    {
        var globals = GlobalParams.ToDictionary(d => d.Name, d => d.Value);
        if (globals.ContainsKey(key))
        {
            return globals[key];
        }

        var lkp = _context.Set<SysParameter>().FirstOrDefault(x => x.Name == key);
        if (lkp != null)
        {
            defaults = lkp.Value;
        }

        return defaults;

    }
}