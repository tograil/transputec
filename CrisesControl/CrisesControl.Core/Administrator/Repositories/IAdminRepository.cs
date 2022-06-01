using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator.Repositories
{
    public interface IAdminRepository
    {
        Task<List<LibIncident>> GetAllLibIncident();
    }
}
