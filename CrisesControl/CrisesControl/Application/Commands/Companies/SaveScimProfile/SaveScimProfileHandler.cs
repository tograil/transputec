using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.SaveScimProfile
{
    public class SaveScimProfileHandler : IRequestHandler<SaveScimProfileRequest, SaveScimProfileResponse>
    {
        private readonly ICompanyQuery _companyQuery;
        private readonly ILogger<SaveScimProfileHandler> _logger;

        public SaveScimProfileHandler(ICompanyQuery companyQuery, ILogger<SaveScimProfileHandler> logger)
        {
            this._companyQuery = companyQuery;
            this._logger = logger;


        }
        public async Task<SaveScimProfileResponse> Handle(SaveScimProfileRequest request, CancellationToken cancellationToken)
        {
            var result = await _companyQuery.SaveScimProfile(request);
            return result;
               
        }
    }
}
