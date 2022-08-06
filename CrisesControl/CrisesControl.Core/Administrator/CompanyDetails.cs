using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class CompanyDetails
    {
        public int CompanyId { get; set; }
        public string Companyname { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyLogoPath { get; set; }
        public string ISDCode { get; set; }
        public string UniqueKey { get; set; }
        public string SwitchBoardPhone { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public int CompanyStatus { get; set; }
        public int Status { get; set; }
        public string CompanyProfile { get; set; }
        public int PackagePlanId { get; set; }
        public string PackagePlanName { get; set; }
        public string PlanName { get; set; }
        public decimal PackagePrice { get; set; }
        public bool OnTrial { get; set; }
        public string InvitationCode { get; set; }
        public string CustomerId { get; set; }
        public string PackagePlanDescription { get; set; }
        public int TotalUsers { get; set; }
        public int TotalActiveUser { get; set; }
        public int TotalInActiveUser { get; set; }
        public int TotalDeletedUser { get; set; }
        public int TotalPendingVerify { get; set; }
        public int TotalAssetCount { get; set; }
        public double TotalAssetSize { get; set; }
        public int TotalLocations { get; set; }
        public int TotalActiveLocation { get; set; }
        public int TotalInactiveLocation { get; set; }
        public int TotalDeletedLocation { get; set; }
        public int TotalGroup { get; set; }
        public int TotalActiveGroup { get; set; }
        public int TotalInactiveGroup { get; set; }
        public int TotalDeletedGroup { get; set; }
        public int TotalDepartment { get; set; }
        public int TotalActiveDepartment { get; set; }
        public int TotalInactiveDepartment { get; set; }
        public int TotalDeletedDepartment { get; set; }
        public int TotalIncidentsConfigured { get; set; }
        public int TotalActiveIncidents { get; set; }
        public int TotalIncidentWithTask { get; set; }
        public int TotalCheckListItem { get; set; }
        public int TotalSOSLaunched { get; set; }
        [NotMapped]
        public PreContractOffer ContractOffer { get; set; }
        public List<AdminTransactionType> TransactionTypes { get; set; }
        [NotMapped]
        public CompanyActivation ActivationKey { get; set; }
        [NotMapped]
        public CompanyPaymentProfile PaymentProfile { get; set; }
        [NotMapped]
        public List<CompanyPackageItem> PackageItems { get; set; }
        [NotMapped]
        public IncidentPingStats CompanyStats { get; set; }
        [NotMapped]
        public RegisteredUser CompanyRegisteredUser { get; set; }
        [NotMapped]
        public CompanyMessageTransactionStats MessageTransactionCount { get; set; }
    }
}
