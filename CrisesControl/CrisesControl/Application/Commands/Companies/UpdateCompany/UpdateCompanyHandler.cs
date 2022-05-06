using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using MediatR;

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

            Company value = _mapper.Map<UpdateCompanyRequest, Company>(request);
            var departmentId = await _companyRepository.UpdateDepartment(value, cancellationToken);
            var result = new UpdateCompanyResponse();
            result.CompanyId = departmentId;
            result.ErrorCode=System.Net.HttpStatusCode.OK;
            return result;
        }
    }
}
