using System.Threading.Tasks;

namespace CrisesControl.Core.CompanyAggregate.Repositories;

public interface IRegisterCompanyRepository
{
    Registration? GetRegistrationDataByEmail(string email);
    Task<string> TempRegister(Registration tempCompany);
    Task<string> UpdateRegistration(Registration tempCompany);
}