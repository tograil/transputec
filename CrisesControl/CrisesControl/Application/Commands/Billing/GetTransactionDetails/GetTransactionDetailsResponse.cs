﻿using CrisesControl.Core.Billing;

namespace CrisesControl.Api.Application.Commands.Billing.GetTransactionDetails
{
    public class GetTransactionDetailsResponse
    {
        public List<TransactionItemDetails> Details { get; set; }
    }
}
