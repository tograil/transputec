using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.CompanyParameters.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyParameterByName
{
    public class GetCompanyParameterByNameHandler : IRequestHandler<GetCompanyParameterByNameRequest, GetCompanyParameterByNameResponse>
    {
        private readonly ICompanyParametersRepository _companyParametersRepository;
        private readonly ILogger<GetCompanyParameterByNameHandler> _logger;
        private readonly GetCompanyParameterByNameValidator _getCompanyParameterByNameValidator;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public GetCompanyParameterByNameHandler(ICompanyParametersRepository companyParametersRepository, ILogger<GetCompanyParameterByNameHandler> logger,
            GetCompanyParameterByNameValidator getCompanyParameterByNameValidator, IMapper mapper, ICurrentUser currentUser)
        {
            this._companyParametersRepository = companyParametersRepository;
            this._logger = logger;
            this._getCompanyParameterByNameValidator = getCompanyParameterByNameValidator;
            this._mapper = mapper;
            this._currentUser = currentUser;
        }
        public async Task<GetCompanyParameterByNameResponse> Handle(GetCompanyParameterByNameRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetCompanyParameterByNameRequest));

            await _getCompanyParameterByNameValidator.ValidateAndThrowAsync(request, cancellationToken);

            var name = await _companyParametersRepository.GetCompanyParameter(request.ParamName, 0, string.Empty, request.CustomerId);
            var result = _mapper.Map<string>(name);
            var response = new GetCompanyParameterByNameResponse();
            if (result!=string.Empty) { 
            response.ParameterName = name;
            return response;
            }
            throw new CompanyNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
        }
    }
}
