using System.Data.SqlTypes;
using CrisesControl.Core.CompanyAggregate;
using CrisesControl.Core.CompanyAggregate.Repositories;
using CrisesControl.Core.UserAggregate;
using CrisesControl.Core.UserAggregate.Repositories;
using CrisesControl.SharedKernel.Utils;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.CompleteRegistration;

public class CompleteRegistrationHandler : IRequestHandler<CompleteRegistrationRequest, CompleteRegistrationResponse>
{
    private readonly IRegisterCompanyRepository _registerCompanyRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserRepository _userRepository;

    public CompleteRegistrationHandler(IRegisterCompanyRepository registerCompanyRepository,
        ICompanyRepository companyRepository,
        IUserRepository userRepository)
    {
        _registerCompanyRepository = registerCompanyRepository;
        _companyRepository = companyRepository;
        _userRepository = userRepository;
    }

    public async Task<CompleteRegistrationResponse> Handle(CompleteRegistrationRequest request, CancellationToken cancellationToken)
    {
        var registration = _registerCompanyRepository.GetRegistrationDataById(request.RegId);

        var newCompany = new Company
        {
            CompanyName = registration.CompanyName!,
            Status = registration.Status,
            CompanyProfile = "AWAITING_SETUP",
            OnTrial = false,
            Isdcode = registration.MobileIsd,
            SwitchBoardPhone = registration.MobileNo,
            RegistrationDate = DateTimeOffset.Now,
            AnniversaryDate = DateTimeOffset.Now.AddMonths(1),
            CompanyLogoPath = string.Empty,
            PackagePlanId = registration.PackagePlanId,
            CreatedOn = DateTime.Now.GetDateTimeOffset(),
            UpdatedOn = DateTime.Now.GetDateTimeOffset(),
            TimeZone = 26,
            CustomerId = registration.CustomerId,
            InvitationCode = Guid.NewGuid().ToString().Replace("-", "").Left(8).ToUpper(),
            Sector = registration.Sector,
            ContactLogoPath = string.Empty
        };

        var newCompanyId = await _companyRepository.CreateCompany(newCompany, cancellationToken);

        throw new NotImplementedException();
    }

    public async Task<int> CreateUser(int companyId, bool registeredUser, string firstName, string primaryEmail, string password,
        int status, int createdUpdatedBy, string timeZoneId, string lastName = "", string mobileNo = "",
        string userRole = "",
        string userPhoto = "no-photo.jpg", string isdCode = "", string llIsdCode = "", string landLine = "",
        string secondaryEmail = "",
        string uniqueGuiD = "", string lat = "", string lng = "", string token = "", bool expirePassword = true,
        string userLanguage = "en",
        bool smsTrigger = false, bool firstLogin = true, int departmentId = 0)
    {


        var checkEmail = DuplicateEmail(primaryEmail);
        if (checkEmail)
            return 0;

        var newUsers = new User
        {
            CompanyId = companyId,
            RegisteredUser = registeredUser,
            FirstName = firstName
        };

        if (!string.IsNullOrEmpty(lastName))
            newUsers.LastName = lastName;

        if (!string.IsNullOrEmpty(isdCode))
        {
            newUsers.Isdcode = isdCode.Left(1) != "+" ? "+" + isdCode : isdCode;
        }

        if (!string.IsNullOrEmpty(mobileNo))
            newUsers.MobileNo = mobileNo.FixMobileZero();

        if (!string.IsNullOrEmpty(llIsdCode))
            newUsers.Llisdcode = llIsdCode.Left(1) != "+" ? "+" + llIsdCode : llIsdCode;

        if (!string.IsNullOrEmpty(landLine))
            newUsers.Landline = landLine.FixMobileZero();

        newUsers.PrimaryEmail = primaryEmail.ToLower();
        newUsers.UserHash = primaryEmail.ToLower().PwdEncrypt();

        if (!string.IsNullOrEmpty(secondaryEmail))
            newUsers.SecondaryEmail = secondaryEmail;

        newUsers.Password = password;

        if (!string.IsNullOrEmpty(uniqueGuiD))
            newUsers.UniqueGuiId = uniqueGuiD;
        else
            newUsers.UniqueGuiId = Guid.NewGuid().ToString();

        newUsers.Status = status;

        if (!string.IsNullOrEmpty(userPhoto))
            newUsers.UserPhoto = userPhoto;

        if (!string.IsNullOrEmpty(userRole))
        {
            newUsers.UserRole = userRole.ToUpper().Replace("STAFF", "USER");
        }
        else
        {
            newUsers.UserRole = "USER";
        }

        if (!string.IsNullOrEmpty(lat))
            newUsers.Lat = lat.Left(15);

        if (!string.IsNullOrEmpty(lng))
            newUsers.Lng = lng.Left(15);

        string compExpirePwd = await _companyRepository.GetCompanyParameter("EXPIRE_PASSWORD", companyId);

        newUsers.ExpirePassword = compExpirePwd == "true" && expirePassword;

        newUsers.UserLanguage = userLanguage;
        newUsers.PasswordChangeDate = DateTime.Now.GetDateTimeOffset(timeZoneId);
        newUsers.FirstLogin = firstLogin;

        newUsers.CreatedBy = createdUpdatedBy;
        newUsers.CreatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);
        newUsers.UpdatedBy = createdUpdatedBy;
        newUsers.UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);
        newUsers.TrackingStartTime = SqlDateTime.MinValue.Value;
        newUsers.TrackingEndTime = SqlDateTime.MinValue.Value;
        newUsers.LastLocationUpdate = SqlDateTime.MinValue.Value;
        newUsers.DepartmentId = departmentId;
        newUsers.Otpexpiry = SqlDateTime.MinValue.Value;

        var roles = Roles.CcRoles(true);

        newUsers.Smstrigger = (roles.Contains(newUsers.UserRole.ToUpper()) && smsTrigger);

        var userId = await _userRepository.CreateUser(newUsers, CancellationToken.None);

        
        if (createdUpdatedBy <= 0)
        {
            var usr = await _userRepository.GetUserById(userId);

            if (usr is not null)
            {
                usr.CreatedBy = newUsers.UserId;
                usr.UpdatedBy = newUsers.UserId;

                await _userRepository.UpdateUser(usr, CancellationToken.None);
            }
        }

        //AddPwdChangeHistory(NewUsers.UserId, password, timeZoneId);

        //CreateUserSearch(NewUsers.UserId, firstName, lastName, ISDCode, mobileNo, primaryEmail, companyId);

        //CreateSMSTriggerRight(companyId, NewUsers.UserId, NewUsers.UserRole, SMSTrigger, NewUsers.ISDCode, NewUsers.mobileNo);

        return newUsers.UserId;

    }

    public bool DuplicateEmail(string strEmailId)
    {
        strEmailId = strEmailId.Trim().ToLower();

        return _userRepository.EmailExists(strEmailId);
    }
}