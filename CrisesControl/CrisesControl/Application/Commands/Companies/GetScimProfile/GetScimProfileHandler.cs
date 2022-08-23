using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetScimProfile
{
    public class GetScimProfileHandler : IRequestHandler<GetScimProfileRequest, GetScimProfileResponse>
    {
        
        private readonly ICompanyQuery _companyQuery;
        private readonly ILogger<GetScimProfileHandler> _logger;

        public GetScimProfileHandler( ICompanyQuery companyQuery, ILogger<GetScimProfileHandler> logger)
        {
           
            _companyQuery = companyQuery;
            _logger = logger;
        }
        public Task<GetScimProfileResponse> Handle(GetScimProfileRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
