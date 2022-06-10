using CC.Authority.Implementation.Data;
using CC.Authority.SCIM.Protocol;
using CC.Authority.SCIM.Schemas;
using CC.Authority.SCIM.Service;

namespace CC.Authority.Implementation.Scim
{
    public class ScimGroupProvider : ProviderBase
    {
        private readonly CrisesControlAuthContext authContext;

        public ScimGroupProvider(CrisesControlAuthContext authContext)
        {
            this.authContext = authContext;
        }

        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
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