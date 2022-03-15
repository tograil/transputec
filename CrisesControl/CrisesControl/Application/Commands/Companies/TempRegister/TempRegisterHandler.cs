using AutoMapper;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.SharedKernel.Utils;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.TempRegister;

public class TempRegisterHandler : IRequestHandler<TempRegisterRequest, string>
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

    public async Task<string> Handle(TempRegisterRequest request, CancellationToken cancellationToken)
    {
        await _tempRegisterValidator.ValidateAndThrowAsync(request, cancellationToken);

        var registration = _registerCompanyService.GetRegistrationDataByEmail(request.Email);

        if (registration == null)
        {
            var newRegistration = _mapper.Map<Registration>(request);

            var registeredId = await _registerCompanyService.TempRegister(newRegistration);

            return registeredId;
        }

        if (!string.IsNullOrEmpty(request.FirstName))
            registration.FirstName = request.FirstName;

        if (!string.IsNullOrEmpty(request.LastName))
            registration.LastName = request.LastName;

        if (!string.IsNullOrEmpty(request.Password))
            registration.Password = request.Password;

        if (!string.IsNullOrEmpty(request.MobileISD))
            registration.MobileIsd = request.MobileISD;

        if (!string.IsNullOrEmpty(request.MobileNo))
            registration.MobileNo = request.MobileNo;

        if (!string.IsNullOrEmpty(request.Sector))
            registration.Sector = request.Sector;

        if (request.RegAction == "CHANGE" && !string.IsNullOrEmpty(request.NewRegEmail))
            registration.Email = request.NewRegEmail;

        if (request.RegAction != "MOBILECHANGE")
        {
            registration.CompanyName = !string.IsNullOrEmpty(request.CompanyName) ? request.CompanyName : ""; ;
            registration.CustomerId = !string.IsNullOrEmpty(request.CustomerId) ? request.CustomerId : ""; ;
            registration.AddressLine1 = !string.IsNullOrEmpty(request.AddressLine1) ? request.AddressLine1 : "";
            registration.AddressLine2 = !string.IsNullOrEmpty(request.AddressLine2) ? request.AddressLine2 : "";
            registration.City = !string.IsNullOrEmpty(request.City) ? request.City : "";
            registration.State = !string.IsNullOrEmpty(request.State) ? request.State : "";
            registration.Postcode = !string.IsNullOrEmpty(request.Postcode) ? request.Postcode : "";
            registration.CountryCode = !string.IsNullOrEmpty(request.CountryCode) ? request.CountryCode : "GBR";
        }

        if (!string.IsNullOrEmpty(request.VerificationCode))
        {
            registration.VerificationCode = request.VerificationCode;
            registration.VerficationExpire = DateTime.Now.AddMinutes(15);
        }
        registration.Status = request.Status;

        if (request.Status != 2)
            registration.UniqueReference = Guid.NewGuid().ToString();

        if (registration.Status == 1 && request.RegAction != "CHANGE")
        {
            registration.CreatedOn = DateTime.Now.GetDateTimeOffset();
        }

        var result = await _registerCompanyService.TempRegister(registration);

        return result;
    }
}