using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.SharedKernel.Utils;
using MediatR;
using Serilog;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompany
{
    public class UpdateCompanyHandler: IRequestHandler<UpdateCompanyRequest, UpdateCompanyResponse>
    {
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<UpdateCompanyHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;
        public UpdateCompanyHandler(ICurrentUser currentUser, IMapper mapper,ILogger<UpdateCompanyHandler> logger,
        ICompanyRepository companyRepository)
        {
            this._currentUser = currentUser;
            this._logger = logger;
            this._companyRepository=companyRepository;
            this._mapper = mapper;
        }

        public async Task<UpdateCompanyResponse> Handle(UpdateCompanyRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateCompanyRequest));            

            try
            {
                Company company = await this._companyRepository.GetCompanyByID(request.CompanyId);
                if (company != null)
                {

                    company.CompanyName = request.CompanyName;

                    company.UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone); ;
                    company.UpdatedBy = _currentUser.UserId;
                    company.CompanyProfile = request.CompanyProfile;
                    company.Website = request.Website;
                    company.SwitchBoardPhone = request.SwitchBoardPhone;
                    company.TimeZone =Int32.Parse( _currentUser.TimeZone);

                    await _companyRepository.UpdateCompany(company);
                   // _mapper.Map<UpdateCompanyRequest, Company>(request);

                    return new UpdateCompanyResponse() { 
                    CompanyId = request.CompanyId,
                    ErrorCode = System.Net.HttpStatusCode.OK,
                    Message= "Company has been updated"

                    };
                }
                return new UpdateCompanyResponse()
                {
                    CompanyId = 0,
                    ErrorCode = System.Net.HttpStatusCode.NotFound,
                    Message = "NOT Found"

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
