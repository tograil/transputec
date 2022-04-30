using System;
using System.Linq;
using CrisesControl.Core.Models;
using CrisesControl.Core.Settings.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.Extensions.Caching.Memory;

namespace CrisesControl.Infrastructure.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private IMemoryCache _memoryCache;
    private CrisesControlContext _context;

    public SettingsRepository(IMemoryCache memoryCache,
        CrisesControlContext context)
    {
        _memoryCache = memoryCache;
        _context = context;
    }

    public string GetSetting(string key, string defaultValue = "")
    {
        var parameter = _memoryCache.GetOrCreate(key, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromSeconds(300);

            return _context.Set<SysParameter>().FirstOrDefault(x => x.Name == key)?
                .Value ?? defaultValue;
        });

        return parameter;
    }
}