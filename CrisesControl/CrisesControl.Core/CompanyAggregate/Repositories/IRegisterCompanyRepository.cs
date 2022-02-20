using System.Threading.Tasks;

namespace CrisesControl.Core.CompanyAggregate.Repositories;

public interface IRegisterCompanyRepository
{
    Task<string> TempRegister(TempCompanyRegister tempCompany);
}