using AutoMapper;
using CrisesControl.Api.Application.Commands.Companies.CheckCompany;
using CrisesControl.Api.Application.Commands.Companies.DeleteCompany;
using CrisesControl.Api.Application.Commands.Companies.GetCommsMethod;
using CrisesControl.Api.Application.Commands.Companies.GetCompany;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query;

public class CompanyQuery : ICompanyQuery {
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger<CompanyQuery> _logger;
    private readonly IMapper _mapper;

    public CompanyQuery(ICompanyRepository companyRepository, IMapper mapper,
        ILogger<CompanyQuery> logger) {
        _mapper = mapper;
        _companyRepository = companyRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<CompanyInfo>> GetCompanyList(int? status, string? companyProfile) {
        var companies = await _companyRepository.GetAllCompanyList(status, companyProfile);

        _logger.LogInformation("Company list return requested");

        return companies.Select(c => {
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
    public async Task<CheckCompanyResponse> CheckCompany(CheckCompanyRequest request)
    {
        var check = _companyRepository.DuplicateCompany(request.CompanyName, request.CountryCode);
        var result= _mapper.Map<bool>(check);
        var response = new CheckCompanyResponse();
        if (result)
        {
            response.Checked = true;
            response.Message = "The company name already registered in the selected country";
            
        }
        else
        {
            response.Checked = true;
            response.Message = "No record found.";
        }
        return response;
    }

    public async Task<DeleteCompanyResponse> DeleteCompany(DeleteCompanyRequest request)
    {
        try
        {
            var check = _companyRepository.DeleteCompanyComplete(request.CompanyId, request.UserId,request.GUID, request.DeleteType);
            var result = _mapper.Map<CompanyInfo>(check);
            var response = new DeleteCompanyResponse();
            if (result!=null)
            {
               
                response.Message = "Deleted";

            }
            else
            {
               
                response.Message = "No record found.";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw new CompanyNotFoundException(request.CompanyId, request.UserId);
        }
    }
}