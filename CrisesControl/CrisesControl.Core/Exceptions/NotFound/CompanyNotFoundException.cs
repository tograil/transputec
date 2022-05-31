using CrisesControl.Core.Exceptions.NotFound.Base;

namespace CrisesControl.Core.Exceptions.NotFound
{
    public class CompanyNotFoundException : NotFoundBaseException
    {
        public CompanyNotFoundException(int companyId, int userId) 
            : base(companyId, userId)
        {

        }

        public override string Message => "Company not found";
    }
}