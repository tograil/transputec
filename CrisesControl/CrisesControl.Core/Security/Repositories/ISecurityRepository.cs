using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CrisesControl.Core.Security
{
    public interface ISecurityRepository
    {
        Task<IEnumerable<CompanySecurityGroup>> GetCompanySecurityGroup(int CompanyID);
    }
}
