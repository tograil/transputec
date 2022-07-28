using AutoMapper;
using CrisesControl.Api.Application.Commands.Companies.CheckCompany;
using CrisesControl.Api.Application.Commands.Companies.CompanyDataReset;
using CrisesControl.Api.Application.Commands.Companies.DeactivateCompany;
using CrisesControl.Api.Application.Commands.Companies.DeleteCompany;
using CrisesControl.Api.Application.Commands.Companies.GetCommsMethod;
using CrisesControl.Api.Application.Commands.Companies.GetCompany;
using CrisesControl.Api.Application.Commands.Companies.GetCompanyAccount;
using CrisesControl.Api.Application.Commands.Companies.GetCompanyComms;
using CrisesControl.Api.Application.Commands.Companies.GetSite;
using CrisesControl.Api.Application.Commands.Companies.GetSocialIntegration;
using CrisesControl.Api.Application.Commands.Companies.ReactivateCompany;
using CrisesControl.Api.Application.Commands.Companies.SaveSite;
using CrisesControl.Api.Application.Commands.Companies.UpdateCompanyComms;
using CrisesControl.Api.Application.Commands.Companies.ViewCompany;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.AddressDetails.Repositories;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Models;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Api.Application.Query;

public class CompanyQuery : ICompanyQuery {
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger<CompanyQuery> _logger;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public CompanyQuery(ICompanyRepository companyRepository, IMapper mapper,
        ILogger<CompanyQuery> logger, ICurrentUser currentUser) {
        _mapper = mapper;
        _companyRepository = companyRepository;
        _logger = logger;
        _currentUser = currentUser;
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

    public async Task<ViewCompanyResponse> ViewCompany(ViewCompanyRequest request)
    {
        try { 
        var Companydata = await _companyRepository.GetCompanyByID(request.CompanyId);
        var AddressInfo = await _companyRepository.GetCompanyAddress(request.CompanyId);
        var result = _mapper.Map<Company>(Companydata);
        var ResultDTO = new ViewCompanyResponse();
        if (result != null)
        {
            ResultDTO.CompanyName = Companydata.CompanyName;
            ResultDTO.CompanyProfile = Companydata.CompanyProfile;
            ResultDTO.CompanyLogo = Companydata.CompanyLogoPath;
            ResultDTO.ContactLogo = Companydata.ContactLogoPath;
            ResultDTO.MasterActionPlan = (Companydata.PlanDrdoc == null || Companydata.PlanDrdoc == "") ? "" : Companydata.PlanDrdoc;
            ResultDTO.Website = (Companydata.Website == null || Companydata.Website == "") ? "" : Companydata.Website;
            ResultDTO.TimeZone = Companydata.TimeZone.ToString();
            ResultDTO.PhoneISDCode = Companydata.Isdcode;
            ResultDTO.SwitchBoardPhone = Companydata.SwitchBoardPhone;
            ResultDTO.AnniversaryDate = Companydata.AnniversaryDate;
            ResultDTO.Fax = Companydata.Fax;
            ResultDTO.OnTrial = Companydata.OnTrial;
            ResultDTO.CustomerId = Companydata.CustomerId;
            ResultDTO.InvitationCode = Companydata.InvitationCode;
        }
        if (AddressInfo != null)
        {
            ResultDTO.AddressLine1 = AddressInfo.Address.AddressLine1;
            ResultDTO.AddressLine2 = AddressInfo.Address.AddressLine2;
            ResultDTO.City = AddressInfo.Address.City;
            ResultDTO.State = AddressInfo.Address.State;
            ResultDTO.Postcode = AddressInfo.Address.Postcode;
            ResultDTO.CountryCode = AddressInfo.Address.CountryCode;
        }
        ResultDTO.ErrorId = 0;
        ResultDTO.Message = "CompanyView";
        return ResultDTO;
        }
        catch (Exception ex)
        {
            throw new CompanyNotFoundException(request.CompanyId, _currentUser.UserId);
        }
    }

    public async Task<GetSiteResponse> GetSite(GetSiteRequest request)
    {
        try
        {
            var site = await _companyRepository.GetSite( request.SiteId, request.CompanyId);
            var sites = await _companyRepository.GetSites(request.CompanyId);
            var result = _mapper.Map<Site>(site);
            var results =  _mapper.Map<List<Site>>(sites);
            var response = new GetSiteResponse();
            if (request.SiteId>0) { 
                    if (result != null)
                    {
                        response.site = result;
                        response.Message = "Site has been Found";
                    }
                  
            }
            else if(results!=null)
            {
                response.sites = results;
                response.Message = "Sites has been Found";
            }
            else
            {

                response.Message = "No record found.";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw new CompanyNotFoundException(request.CompanyId, _currentUser.UserId);
        }
    }

    public async Task<SaveSiteResponse> SaveSite(SaveSiteRequest request)
    {
        try
        {
            var response = new SaveSiteResponse();
            DateTimeOffset CRTime = DateTime.Now.GetDateTimeOffset( _currentUser.TimeZone);
            if (request.SiteID > 0)
            {
                var site = await _companyRepository.GetCompanySiteById(request.SiteID,request.CompanyID);
                if (site != null)
                {
                    site.SiteName = request.SiteName;
                    site.SiteCode = request.SiteCode;
                    site.UpdatedOn = CRTime;
                    site.UpdatedBy = _currentUser.UserId;
                    site.Status = request.Status;
                    var SiteID = await _companyRepository.SaveSite(site);
                    
                    response.SiteId = SiteID;
                    response.Message = "Updated Site";
                   
                }
            }
            else
            {
                Site ST = new Site();
                ST.CompanyId = request.CompanyID;
                ST.SiteName = request.SiteName;
                ST.SiteCode = request.SiteCode;
                ST.Status = request.Status;
                ST.CreatedBy = _currentUser.UserId;
                ST.CreatedOn = CRTime;
                ST.UpdatedBy = _currentUser.UserId;
                ST.UpdatedOn = CRTime;
                var SiteID = await _companyRepository.SaveSite(ST);

                response.SiteId = SiteID;
                response.Message = "Added Successfull";
               
            }
            return response;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<GetSocialIntegrationResponse> GetSocialIntegration(GetSocialIntegrationRequest request)
    {
        try
        {
            var socials = await _companyRepository.GetSocialIntegration(request.CompanyID, request.AccountType);
            var result = _mapper.Map<List<SocialIntegraion>>(socials);
            var response = new GetSocialIntegrationResponse();
            if (result != null)
            {
                response.Data = result;
                response.Message = "Socials has been Found";

            }
            else
            {

                response.Message = "No record found.";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw new CompanyNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
        }
    }

    public async Task<GetCompanyCommsResponse> GetCompanyComms(GetCompanyCommsRequest request)
    {
        try
        {
            var comms = await _companyRepository.GetCompanyComms(_currentUser.CompanyId, _currentUser.UserId);
            var result = _mapper.Map<CompanyCommunication>(comms);
            var response = new GetCompanyCommsResponse();
            if (result != null)
            {
                response.Comms = result;
               

            }
            else
            {

                response.Comms = result;
            }
            return response;

        }
        catch (Exception ex)
        {
            throw new CompanyNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
        }
    }

    public async Task<GetCompanyAccountResponse> GetCompanyAccount(GetCompanyAccountRequest request)
    {
        try
        {
            var comms = await _companyRepository.GetCompanyAccount(_currentUser.CompanyId);
            var result = _mapper.Map<CompanyAccount>(comms);
            var response = new GetCompanyAccountResponse();
            if (result != null)
            {
                response.Account = result;
                response.Message = "Data Loaded";


            }
            else
            {

                response.Account = result;
                response.Message = "No Data found";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw new CompanyNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
        }
    }

    public async Task<UpdateCompanyCommsResponse> UpdateCompanyComms(UpdateCompanyCommsRequest request)
    {
        try
        {
            var update = await _companyRepository.UpdateCompanyComms(_currentUser.CompanyId, request.MethodId, request.BillingUsers, request.CurrentSuperUser, _currentUser.UserId, _currentUser.TimeZone, request.Source); 
            var result = _mapper.Map<ReplyChannel>(update);
            var response = new UpdateCompanyCommsResponse();
            if (result != null)
            {
                response.Data = result;
                response.Message = "updated";


            }
            else
            {

                response.Data = result;
                response.Message = "Not Updated";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw new CompanyNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
        }
    }

    public async Task<DeactivateCompanyResponse> DeactivateCompany(DeactivateCompanyRequest request)
    {
        try
        {
            var response = new DeactivateCompanyResponse();
            var CompanyToDelete =await _companyRepository.GetCompanyByID(request.TargetCompanyID);
            if (CompanyToDelete != null)
            {
                CompanyToDelete.Status = 0;
                CompanyToDelete.UpdatedBy = _currentUser.UserId;
                CompanyToDelete.UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);
                var updateId = await _companyRepository.DeactivateCompany(CompanyToDelete);

                response.CompanyId = updateId;
                response.Message = "Company Deactivated successfully";
            }
            else
            {
                response.CompanyId = 0;
                response.Message = "No record found to delete.";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw new CompanyNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
        }
    }

    public async Task<ReactivateCompanyResponse> ReactivateCompany(ReactivateCompanyRequest request)
    {

        try
        {
            var response = new ReactivateCompanyResponse();
            var CompanyToReactivate = await _companyRepository.GetCompanyByID(request.ActivateReactivateCompanyId);
            if (CompanyToReactivate != null)
            {
                CompanyToReactivate.Status = 1;
                CompanyToReactivate.UpdatedBy = _currentUser.UserId;
                CompanyToReactivate.UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);
                int ActivatedCompnayId = await _companyRepository.ReactivateCompany(CompanyToReactivate);

                response.CompanyId = ActivatedCompnayId;
                response.Message = "Company reactivated successfully";

            }
            else
            {
                response.CompanyId = 0;
                response.Message = "No record found to delete.";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw new CompanyNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
        }
    }

    public async Task<CompanyDataResetResponse> CompanyDataReset(CompanyDataResetRequest request)
    {
        try
        {
            var dataReset = await _companyRepository.CompanyDataReset(request.ResetOptions,_currentUser.CompanyId, _currentUser.TimeZone);
            var result = _mapper.Map<bool>(dataReset);
            var response = new CompanyDataResetResponse();
            if (result != null)
            {
                response.IsReset = result;
                response.Message = "updated";


            }
            else
            {

                response.IsReset = result;
                response.Message = "Not Updated";
            }
            return response;

        }
        catch (Exception ex)
        {
            throw new CompanyNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
        }
    }
}