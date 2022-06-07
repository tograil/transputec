using CrisesControl.Core.Exceptions.NotFound.Base;

namespace CrisesControl.Core.Exceptions.NotFound
{
    public class CompanyNotFoundException : NotFoundRegisterException
    {
        public CompanyNotFoundException(int companyId, int userId) 
            : base(companyId, userId)
        {

        }

        public override string Message => "Company not found";
    }
}