using CrisesControl.Core.Companies;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Models;
using CrisesControl.Core.Payments.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class PaymentRepository: IPaymentRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<PaymentRepository> _logger;
        public PaymentRepository(CrisesControlContext context, ILogger<PaymentRepository> logger)
        {
            this._context = context;
            this._logger = logger;
        }
        public async Task<dynamic> GetCompanyByKey(string ActivationKey, int OutUserCompanyId)
        {
            var cp = await _context.Set<Company>().Include(CA => CA.CompanyActivation)
                                    .Where(CA => CA.CompanyActivation.ActivationKey == ActivationKey && CA.CompanyId == OutUserCompanyId && CA.Status == 0
                                    ).FirstOrDefaultAsync();
       
            return cp;
        }
        public async Task<int> UpgradeByKey(Company company)
        {
           
                _context.Update(company);
              await  _context.SaveChangesAsync();
            _logger.LogInformation("update payment for " + company.CompanyActivation.ActivationKey);
            return company.CompanyId;
              
                
            
        }
        public async Task<bool> OnTrialStatus(string CompanyProfile, bool CurrentTrial)
        {
            if (CompanyProfile == "SUBSCRIBED")
            {
                return false;
            }
            return CompanyProfile == "ON_TRIAL" ? true : CurrentTrial;
        }
    }
}
