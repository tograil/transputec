using CrisesControl.Core.Companies;
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
        Task<Registration> GetRegistrationById(string UniqueRef);
    }
}
