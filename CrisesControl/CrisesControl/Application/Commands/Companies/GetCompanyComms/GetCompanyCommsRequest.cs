﻿using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetCompanyComms
{
    public class GetCompanyCommsRequest:IRequest<GetCompanyCommsResponse>
    {
        public int CompanyId { get; set; }
    }
}
