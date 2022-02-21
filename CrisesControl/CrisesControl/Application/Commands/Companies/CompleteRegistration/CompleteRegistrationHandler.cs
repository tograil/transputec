using CrisesControl.Core.CompanyAggregate;
using CrisesControl.Core.CompanyAggregate.Repositories;
using CrisesControl.SharedKernel.Utils;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.CompleteRegistration;

public class CompleteRegistrationHandler : IRequestHandler<CompleteRegistrationRequest, CompleteRegistrationResponse>
{
    private readonly IRegisterCompanyRepository _registerCompanyRepository;
    private readonly ICompanyRepository _companyRepository;

    public CompleteRegistrationHandler(IRegisterCompanyRepository registerCompanyRepository, ICompanyRepository companyRepository)
    {
        _registerCompanyRepository = registerCompanyRepository;
        _companyRepository = companyRepository;
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
}