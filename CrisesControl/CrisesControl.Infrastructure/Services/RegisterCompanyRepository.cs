using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CrisesControl.Core.CompanyAggregate;
using CrisesControl.Core.CompanyAggregate.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Infrastructure.Services;

public class RegisterCompanyRepository : IRegisterCompanyRepository
{
    private readonly CrisesControlContext _context;
    private readonly IMapper _mapper;

    public RegisterCompanyRepository(CrisesControlContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<string> TempRegister(TempCompanyRegister tempCompany)
    {
       var regDb = _context.Set<Registration>().FirstOrDefault(x => x.Email == tempCompany.Email);

       if (regDb is null)
       {
           var registration = _mapper.Map<Registration>(tempCompany);

           _context.Add(registration);

           await _context.SaveChangesAsync();

           return registration.UniqueReference;
       }
       else
       {
            if (!string.IsNullOrEmpty(tempCompany.FirstName))
                regDb.FirstName = tempCompany.FirstName;

            if (!string.IsNullOrEmpty(tempCompany.LastName))
                regDb.LastName = tempCompany.LastName;

            if (!string.IsNullOrEmpty(tempCompany.Password))
                regDb.Password = tempCompany.Password;

            if (!string.IsNullOrEmpty(tempCompany.MobileISD))
                regDb.MobileIsd = tempCompany.MobileISD;

            if (!string.IsNullOrEmpty(tempCompany.MobileNo))
                regDb.MobileNo = tempCompany.MobileNo;

            if (!string.IsNullOrEmpty(tempCompany.Sector))
                regDb.Sector = tempCompany.Sector;

            if (tempCompany.RegAction == "CHANGE" && !string.IsNullOrEmpty(tempCompany.NewRegEmail))
                regDb.Email = tempCompany.NewRegEmail;

            if (tempCompany.RegAction != "MOBILECHANGE")
            {
                regDb.CompanyName = !string.IsNullOrEmpty(tempCompany.CompanyName) ? tempCompany.CompanyName : ""; ;
                regDb.CustomerId = !string.IsNullOrEmpty(tempCompany.CustomerId) ? tempCompany.CustomerId : ""; ;
                regDb.AddressLine1 = !string.IsNullOrEmpty(tempCompany.AddressLine1) ? tempCompany.AddressLine1 : "";
                regDb.AddressLine2 = !string.IsNullOrEmpty(tempCompany.AddressLine2) ? tempCompany.AddressLine2 : "";
                regDb.City = !string.IsNullOrEmpty(tempCompany.City) ? tempCompany.City : "";
                regDb.State = !string.IsNullOrEmpty(tempCompany.State) ? tempCompany.State : "";
                regDb.Postcode = !string.IsNullOrEmpty(tempCompany.Postcode) ? tempCompany.Postcode : "";
                regDb.CountryCode = !string.IsNullOrEmpty(tempCompany.CountryCode) ? tempCompany.CountryCode : "GBR";
            }

            if (!string.IsNullOrEmpty(tempCompany.VerificationCode))
            {
                regDb.VerificationCode = tempCompany.VerificationCode;
                regDb.VerficationExpire = DateTime.Now.AddMinutes(15);
            }
            regDb.Status = tempCompany.Status;

            if (tempCompany.Status != 2)
                regDb.UniqueReference = Guid.NewGuid().ToString();

            if (regDb.Status == 1 && tempCompany.RegAction != "CHANGE")
            {
                regDb.CreatedOn = DateTime.Now.GetDateTimeOffset();
            }

            await _context.SaveChangesAsync();

            return regDb.UniqueReference;
       }
    }
}