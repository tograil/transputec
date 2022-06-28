using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.CompanyParameters.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.UpdateCompanyParameters
{
    public class UpdateCompanyParametersHandler : IRequestHandler<UpdateCompanyParametersRequest, UpdateCompanyParametersResponse>
    {
        private readonly ICompanyParametersRepository _companyParametersRespository;
        private readonly ILogger<UpdateCompanyParametersHandler> _logger;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        public UpdateCompanyParametersHandler(ICompanyParametersRepository companyParametersRespository, ICurrentUser currentUser, ILogger<UpdateCompanyParametersHandler> logger, IMapper mapper)
        {
            this._companyParametersRespository = companyParametersRespository;
            this._logger = logger;
            this._currentUser = currentUser;
            this._mapper = mapper;
        }
        public async Task<UpdateCompanyParametersResponse> Handle(UpdateCompanyParametersRequest request, CancellationToken cancellationToken)
        {
           var save= await _companyParametersRespository.SaveParameter(request.CompanyParametersId, request.Name, request.Value, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
            var result = _mapper.Map<bool>(save);
            var response = new UpdateCompanyParametersResponse();
            if (result)
            {
                response.Result = result;
                response.Message = "Data Added";
            }
            else
            {
                response.Result = result;
                response.Message = "Data Not Added Succesfully";
            }
                return response;
        }
    }
}
