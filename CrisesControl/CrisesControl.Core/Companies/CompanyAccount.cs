using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Companies
{
    public class CompanyAccount
    {
        public int CompanyId { get; set; }
        public string Companyname { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public int CompanyStatus { get; set; }
        public string CompanyProfile { get; set; }
        public int PackagePlanId { get; set; }
        public string PackagePlanName { get; set; }
        public decimal PackagePrice { get; set; }
        public bool OnTrial { get; set; }
        public PreContractOffer ContractOffer { get; set; }
        public List<CompanyTransactionType> TransactionTypes { get; set; }
        public CompanyActivation ActivationKey { get; set; }
        public CompanyPaymentProfile PaymentProfile { get; set; }
        public List<CompanyPackageItem> PackageItems { get; set; }
    }
}
