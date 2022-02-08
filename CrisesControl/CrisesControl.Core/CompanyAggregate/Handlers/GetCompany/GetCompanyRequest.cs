﻿using MediatR;

namespace CrisesControl.Core.CompanyAggregate.Handlers.GetCompany
{
    public class GetCompanyRequest : IRequest<GetCompanyResponse>
    {
        public int CompanyId { get; set; }
    }
}