using AutoMapper;
using CrisesControl.Api.Application.Commands.Companies.GetCommsMethod;
using CrisesControl.Api.Application.Commands.Companies.GetCompany;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query;

public class CompanyQuery : ICompanyQuery {
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CompanyQuery> _logger;

    public CompanyQuery(ICompanyRepository companyRepository, ILogger<CompanyQuery> logger)
    {
        _companyRepository = companyRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<CompanyInfo>> GetCompanyList(int? status, string? companyProfile) {
        var companies = await _companyRepository.GetAllCompanyList(status, companyProfile);

        return companies.Select(c =>
        {
            var user = c.Users.First();
            var companyPaymentProfile = c.CompanyPaymentProfiles?.FirstOrDefault();

            return new CompanyInfo {
                CompanyId = c.CompanyId,
                CompanyName = c.CompanyName,
                FirstName = user.FirstName,
                LastName = user.LastName ?? string.Empty,
                PrimaryEmail = user.PrimaryEmail,
                AgreementNo = companyPaymentProfile?.AgreementNo ?? string.Empty,
                CompanyLogo = c.CompanyLogoPath ?? string.Empty,
                CompanyProfile = c.CompanyProfile ?? string.Empty,
                ContractAnniversary = companyPaymentProfile?.ContractAnniversary ?? c.RegistrationDate,
                RegistrationDate = c.RegistrationDate,
                CustomerId = c.CustomerId ?? string.Empty,
                InvitationCode = c.InvitationCode ?? string.Empty,
                IsdCode = c.Isdcode ?? string.Empty,
                MobileNo = c.SwitchBoardPhone ?? string.Empty,
                OnTrial = c.OnTrial,
                PlanName = c.PackagePlan.PlanName,
                Status = c.Status,
                SwitchBoardPhone = c.SwitchBoardPhone ?? string.Empty,
            };
        }).ToArray();
    }

    public async Task<GetCompanyResponse> GetCompany(GetCompanyRequest request, CancellationToken cancellationToken) {

        var companyRequest = _mapper.Map<CompanyRequestInfo>(request);
        var companyInfo = await _companyRepository.GetCompany(companyRequest, cancellationToken);
        var result = _mapper.Map<GetCompanyResponse>(companyInfo);
        return result;
    }

    public async Task<GetCommsMethodResponse> GetCommsMethod(CancellationToken cancellationToken) {
        var methods = await _companyRepository.GetCommsMethod();
        var response = _mapper.Map<List<CommsMethod>>(methods);
        var result = new GetCommsMethodResponse();
        result.Data = response;
        result.ErrorCode = "0";
        return result;
    }
}