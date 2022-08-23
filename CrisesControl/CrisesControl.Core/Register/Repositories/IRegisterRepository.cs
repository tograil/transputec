using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
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
        Task<string> ValidateMobile(string Code, string ISD, string MobileNo, string Message = "");
        Task<CommsStatus> SendText(string ISD, string ToNumber, string Message, string CallbackUrl = "");
        Task<CommsStatus> VerificationCall(string mobileNo, string message, bool sendInDirect, string twilioRoutingApi);
        Task<User> ValidateUserEmail(string uniqueId, int companyId);
        Task UpdateUserDepartment(int DepartmentID, int CreatedUpdatedBy, string TimeZoneId);
        Task CreateObjectRelationship(int TargetObjectID, int SourceObjectID, string RelationName, int CompanyId, int CreatedUpdatedBy, string TimeZoneId, string RelatinFilter = "");
        Task CreateNewObjectRelation(int SourceObjectID, int TargetObjectID, int ObjMapId, int CreatedUpdatedBy, string TimeZoneId);
        Task NewUserAccountConfirm(string EmailId, string UserName, string UserPass, int CompanyId, string guid);
        Task<bool> UpgradeRequest(int CompanyId);
        Task<int> VerifyTempRegistration(Registration reg);
        Task<Registration> GetRegistrationByUniqueReference(string UniqueRef);
        Task<Registration> GetRegistrationDataByEmail(string Email);
        Task<Registration> TempRegister(Registration reg);
        Task<bool> SetupCompleted(Company company);
        Task<Registration> GetTempRegistration(int RegID, string UniqueRef);
        Task<bool> DeleteTempRegistration(Registration registration);
        Task<bool> ActivateCompany(int UserId, string ActivationKey, string IPAddress, int SalesSource = 0, string TimeZoneId = "GMT Standard Time");
        Task<UserDevice> GetUserDeviceByUserId(int UserID);
        Task<CompanyUser> SendVerification(string UniqueId);
        Task NewUserAccount(string EmailId, string UserName, int CompanyId, string guid);
        Task<string> UserName(UserFullName strUserName);
        Task SendCredentials(string EmailId, string UserName, string UserPass, int CompanyId, string guid);
        Task<User> GetUserByUniqueId(string UniqueId);
        Task UpdateTemp(User user);
        Task<List<Registration>> GetAllRegistrations();
        Task<List<Sectors>> GetSectors();
        Task<List<PackageModel>> GetAllPackagePlan();
        Task<bool> UpdateCompanyStatus(ViewCompanyModel companyModel);
        Task<UserValidatedDTO> CompleteRegistration(TempRegister tempRegister);
        Task<bool> DeleteTempRegistration(TempRegister tempRegister);
        Task<List<Sectors>> BusinessSector();
    }
}
