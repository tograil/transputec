using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Security;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.GetAllSecurityObjects
{
    public class GetAllSecurityObjectsHandler : IRequestHandler<GetAllSecurityObjectsRequest, GetAllSecurityObjectsResponse>
    {
        private readonly ISecurityRepository _securityRepository;
        private readonly ICurrentUser _currentUser;
        public GetAllSecurityObjectsHandler(ISecurityRepository securityRepository, ICurrentUser currentUser)
        {
            this._securityRepository = securityRepository;
            this._currentUser = currentUser;
        }
        public async Task<GetAllSecurityObjectsResponse> Handle(GetAllSecurityObjectsRequest request, CancellationToken cancellationToken)
        {
            var allObjects = await _securityRepository.GetAllSecurityObjects(_currentUser.CompanyId);
            var response = new GetAllSecurityObjectsResponse();
            if (allObjects != null)
            {
                response.Data = allObjects;  
            }
            else
            {
                response.Data = new List<SecurityAllObjects>();
            }
            return response;
        }
    }
}
