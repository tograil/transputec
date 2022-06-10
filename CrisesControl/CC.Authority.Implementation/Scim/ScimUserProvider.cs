using CC.Authority.Implementation.Data;
using CC.Authority.SCIM.Protocol;
using CC.Authority.SCIM.Schemas;
using CC.Authority.SCIM.Service;
using Microsoft.EntityFrameworkCore;

namespace CC.Authority.Implementation.Scim
{
    public class ScimUserProvider : ProviderBase
    {
        private readonly CrisesControlAuthContext _authContext;

        public ScimUserProvider(CrisesControlAuthContext authContext)
        {
            this._authContext = authContext;
        }

        public override async Task<Resource[]> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            return await _authContext.Users.Select(user => new Core2EnterpriseUser
            {
                Active = true,
                DisplayName = $"{user.FirstName} {user.LastName}",
                Roles = new []{ new Role
                {
                    Display = user.UserRole!
                } }
            }).ToArrayAsync();
        }

        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            var user = resource as Core2EnterpriseUser;

            throw new NotImplementedException();
        }

        public override Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            throw new NotImplementedException();
        }

        public override Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateAsync(IPatch patch, string correlationIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}