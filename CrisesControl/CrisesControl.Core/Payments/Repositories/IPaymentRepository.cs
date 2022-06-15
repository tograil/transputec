using CrisesControl.Core.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Payments.Repositories
{
    public interface IPaymentRepository
    {
        Task<Company> GetCompanyByKey(string ActivationKey, int OutUserCompanyId);
        Task<bool> OnTrialStatus(string CompanyProfile, bool CurrentTrial);
        Task<int> UpgradeByKey(Company company);
    }
}
