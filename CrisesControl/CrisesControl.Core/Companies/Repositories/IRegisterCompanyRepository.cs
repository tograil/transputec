using System.Threading.Tasks;

namespace CrisesControl.Core.Companies.Repositories;

public interface IRegisterCompanyRepository
{
    Registration? GetRegistrationDataByEmail(string email);
    Registration GetRegistrationDataById(int id);
    Task<string> TempRegister(Registration tempCompany);
    Task<string> UpdateRegistration(Registration tempCompany);
}