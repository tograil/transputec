using System.Collections.Generic;

namespace CrisesControl.Core.CompanyAggregate.Repositories;

public interface IGlobalParametersRepository
{
    IEnumerable<GlobalParams> GlobalParams { get; }
    string LookupWithKey(string key, string defaults = "");
}