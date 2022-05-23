using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using MediatR;
using Serilog;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyLogo
{
    public class UpdateCompanyLogoHandler : IRequestHandler<UpdateCompanyLogoRequest, UpdateCompanyLogoResponse>
    {
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<UpdateCompanyLogoHandler> _logger;
        private readonly ICompanyRepository _companyRepository;
        public UpdateCompanyLogoHandler(ICurrentUser currentUser, ILogger<UpdateCompanyLogoHandler> logger, ICompanyRepository companyRepository)
        {
            this._currentUser = currentUser;
            this._logger = logger;
            this._companyRepository = companyRepository;
        }
        public async Task<UpdateCompanyLogoResponse> Handle(UpdateCompanyLogoRequest request, CancellationToken cancellationToken)
        {
            try
            {
                Company company = await this._companyRepository.GetCompanyByID(request.CompanyId);
                if (company != null)
                {
                    if (request.LogoType.ToLTString().ToUpper() == LogoType.EMAILLOGO.ToLTString().ToUpper() )
                    {
                        company.CompanyLogoPath = request.CompanyLogo;
                        company.IOslogo = request.iOSLogo;
                        company.AndroidLogo = request.AndroidLogo;
                        company.WindowsLogo = request.WindowsLogo;
                    }
                    else
                    {
                        company.ContactLogoPath = request.CompanyLogo;
                    }
                    company.UpdatedBy = _currentUser.UserId;
                    company.UpdatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(_currentUser.TimeZone);

                    await _companyRepository.UpdateCompanyLogo(company);

                    return new UpdateCompanyLogoResponse
                    {
                        CompanyId = request.CompanyId,
                        CompanyLogo = (company.CompanyLogoPath == null || company.CompanyLogoPath == "") ? "" : company.CompanyLogoPath,
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Message = "Company Logo has been uploaded successfully"
                    };
                }
                return new UpdateCompanyLogoResponse
                {
                    Message = "Company not found",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return null;
            }
            
        }
    }
}
