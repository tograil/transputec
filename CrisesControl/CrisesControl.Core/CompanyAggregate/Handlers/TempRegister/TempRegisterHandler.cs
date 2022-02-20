using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CrisesControl.Core.CompanyAggregate.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Core.CompanyAggregate.Handlers.TempRegister;

public class TempRegisterHandler : IRequestHandler<TempRegisterRequest, TempRegisterResponse>
{
    private readonly TempRegisterValidator _tempRegisterValidator;
    private readonly IRegisterCompanyRepository _registerCompanyService;
    private readonly IMapper _mapper;

    public TempRegisterHandler(IRegisterCompanyRepository registerCompanyService,
        IMapper mapper,
        TempRegisterValidator tempRegisterValidator)
    {
        _registerCompanyService = registerCompanyService;
        _mapper = mapper;
        _tempRegisterValidator = tempRegisterValidator;
    }

    public async Task<TempRegisterResponse> Handle(TempRegisterRequest request, CancellationToken cancellationToken)
    {
        await _tempRegisterValidator.ValidateAndThrowAsync(request, cancellationToken);

        var tempCompanyRegisterRoot = _mapper.Map<TempCompanyRegister>(request);

        var result = await _registerCompanyService.TempRegister(tempCompanyRegisterRoot);

        return new TempRegisterResponse
        {
            Result = result
        };
    }
}