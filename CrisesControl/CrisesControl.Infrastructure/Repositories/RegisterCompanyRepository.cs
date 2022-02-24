using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CrisesControl.Core.CompanyAggregate;
using CrisesControl.Core.CompanyAggregate.Repositories;
using CrisesControl.Infrastructure.Context;

namespace CrisesControl.Infrastructure.Repositories;

public class RegisterCompanyRepository : IRegisterCompanyRepository
{
    private readonly CrisesControlContext _context;
    private readonly IMapper _mapper;

    public RegisterCompanyRepository(CrisesControlContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Registration? GetRegistrationDataByEmail(string email)
    {
        var regDb = _context.Set<Registration>().FirstOrDefault(x => x.Email == email);

        return regDb;
    }

    public async Task<string> TempRegister(Registration tempCompany)
    {
       _context.Add(tempCompany);

       await _context.SaveChangesAsync();

       return tempCompany.UniqueReference;
    }

    public async Task<string> UpdateRegistration(Registration tempCompany)
    {
        _context.Update(tempCompany);

        await _context.SaveChangesAsync();
        
        return tempCompany.UniqueReference;
    }
}