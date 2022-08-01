using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public class UsageHelper
    {
        private readonly CrisesControlContext _context;
        public UsageHelper(CrisesControlContext context)
        {
            _context = context;
        }

        public async Task<int> AddTransactionHeader(int CompanyId, decimal NetTotal, decimal VatRate, decimal VatAmount, decimal Total, decimal CreditBalance, decimal CreditLimit,
           int AdminLimit, int AdminUsers, int StaffLimit, int StaffUsers, int StorageLimit, double StorageSize, DateTimeOffset StatementStartDate, DateTimeOffset StatementEndDate)
        {
            try
            {
                TransactionHeader NewTransactionHeader = new TransactionHeader()
                {
                    CompanyId = CompanyId,
                    NetTotal = NetTotal,
                    VatRate = VatRate,
                    Vatvalue = VatAmount,
                    Total = Total,
                    CreditBalance = CreditBalance,
                    CreditLimit = CreditLimit,
                    AdminLimit = AdminLimit,
                    AdminUsers = AdminUsers,
                    StaffLimit = StaffLimit,
                    StaffUsers = StaffUsers,
                    StorageLimit = StorageLimit,
                    StorageSize = StorageSize,
                    StatementDate = DateTime.Now,
                    TransactionStartDate = StatementStartDate,
                    TransactionEndDate = StatementEndDate,
                    CreatedBy = 0,
                    CreatedOn = DateTime.Now,
                    UpdatedBy = 0,
                    UpdatedOn = DateTime.Now
                };
                _context.Set<TransactionHeader>().Add(NewTransactionHeader);
                await _context.SaveChangesAsync();
                return NewTransactionHeader.TransactionHeaderId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
