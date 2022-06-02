using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Register.Repositories
{
    public interface IRegisterRepository
    {
        Task<bool> CheckCustomer(string CustomerId);
    }
}
